using KivalitaAPI.Models;
using KivalitaAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using KivalitaAPI;
using System.Web;
using System.Text;
using KivalitaAPI.Repositories;
using KivalitaAPI.Enum;
using System.Globalization;

[DisallowConcurrentExecution]
public class FindMailJob : IJob
{
    private readonly ILogger<FindMailJob> _logger;
    IServiceProvider _serviceProvider;
    IServiceScope scope;
    private Semaphore semaphore;

    public FindMailJob(ILogger<FindMailJob> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        this._serviceProvider = serviceProvider;
        semaphore = new Semaphore(1, 1);
    }

    public Task Execute(IJobExecutionContext context)
    {
        var thread = new Thread(async () => { await workerTask(); });
        thread.Start();
        return Task.CompletedTask;
    }

    public async Task workerTask()
    {
        try
        {
            semaphore.WaitOne();
            _logger.LogInformation($"{DateTime.Now}");
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var leadsRepository = scope.ServiceProvider.GetService<LeadsRepository>();
            var leads = leadsRepository.GetBy(l => l.DidGuessEmail == false && l.CompanyId != null);

            var emailExtractorService = scope.ServiceProvider.GetService<EmailExtractorService>(); 
            List<Leads> leadEmailGuessed = new List<Leads>();

            foreach (var lead in leads)
            {
                string firstName;
                string lastName;

                var domains = GetDomainsFromCompany(lead.Company);
                (firstName, lastName) = GetNames(lead.Name);

                Console.WriteLine($"{lead.Company.Name} - {firstName} {lastName}");

                string email;
                try
                {
                    email = await emailExtractorService.Run(firstName, lastName, domains);
                }
                catch
                {
                    email = null;
                }

                Console.WriteLine($"Email - {email}");

                lead.Email = (!String.IsNullOrEmpty(email)) ? email : lead.Email;
                lead.DidGuessEmail = true;
                leadEmailGuessed.Add(lead);
            }

            leadsRepository.UpdateRangeNoMapper(leadEmailGuessed);
        }
        catch(Exception e)
        {
            _logger.LogError($"{e.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }

    public List<string> GetDomainsFromCompany(Company company)
    {
        List<string> domains = new List<string> { };

        if (String.IsNullOrEmpty(company.Site))
        {
            var names = GetNames(company.Name, true);

            domains.Add($"{names.Item1}.com");
            domains.Add($"{names.Item1}.com.br");
            domains.Add($"{names.Item1}{names.Item2}.com");
            domains.Add($"{names.Item1}{names.Item2}.com.br");
        }
        else
        {
            var url = company.Site;

            if (!url.StartsWith("http"))
            {
                url = $"https://{url}";
            }
            string domain = new Uri(url).Host;
            if (domain.StartsWith("www"))
            {
                domain = domain.Remove(0, 4);
            }

            domains.Add(domain);

        }

        domains = domains.Distinct().ToList();
        return domains;
    }

    public (string, string) GetNames(string fullName, bool isCompany = false)
    {
        fullName = RemoveAccentuation(fullName);

        List<string> names = fullName.ToLower().Split(' ').ToList();
        List<string> namesFiltered = names.FindAll(n => n.Length > 2);

        switch (namesFiltered.Count)
        {
            case 0:
                return ("", "");
            case 1:
                return (namesFiltered[0], "");
            case 2:
                return (namesFiltered[0], namesFiltered[1]);
            default:
                if (isCompany)
                {
                    return (namesFiltered[0], namesFiltered[1]);
                }
                else
                {
                    return (namesFiltered[0], namesFiltered[namesFiltered.Count - 1]);
                }
        }
    }

    static string RemoveAccentuation(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}