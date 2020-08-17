using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace KivalitaAPI.Services
{
    public class GetEmailService
    {
        IEmailExtractorService emailExtractorService;
        IServiceProvider _serviceProvider;
        IServiceScope scope;

        public GetEmailService(IServiceProvider serviceProvider, IEmailExtractorService emailExtractorService)
        {
            this.emailExtractorService = emailExtractorService;
            this._serviceProvider = serviceProvider;
        }

        public async Task FromLeadIdAsync(int leadId)
        {
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            var leadsRepository = scope.ServiceProvider.GetService<LeadsRepository>();
            var lead = leadsRepository.Get(leadId);

            await FromLeadAsync(lead);

        }

        public async Task FromLeadAsync(Leads lead)
        {
            if (scope == null)
            {
                scope = this._serviceProvider.CreateScope();
            }

            if (!lead.CompanyId.HasValue)
            {
                return;
            }

            var leadsRepository = scope.ServiceProvider.GetService<LeadsRepository>();

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

            lead.Email = email;
            lead.DidGuessEmail = true;
            leadsRepository.Update(lead);
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
}
