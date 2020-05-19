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

            if (String.IsNullOrEmpty(lead.CompanySite))
            {
                return;
            }

            var leadsRepository = scope.ServiceProvider.GetService<LeadsRepository>();

            string firstName;
            string lastName;
            string domain;

            domain = GetDomain(lead.CompanySite);
            (firstName, lastName) = GetFirstAndLastName(lead.Name);

            Console.WriteLine($"{domain} - {firstName} {lastName}");

            string email;
            try
            {
                email = await emailExtractorService.Run(firstName, lastName, domain);
            }
            catch
            {
                email = null;
            }

            Console.WriteLine($"Email - {email}");

            if (email != null)
            {
                lead.Email = email;
                leadsRepository.Update(lead);
            }

        }

        public string GetDomain(string url)
        {
            if (!url.StartsWith("http"))
            {
                url = $"https://{url}";
            }
            string domain = new Uri(url).Host;
            if (domain.StartsWith("www"))
            {
                domain = domain.Remove(0, 4);
            }

            return domain;
        }

        public (string, string) GetFirstAndLastName(string fullName)
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
                    return (namesFiltered[0], namesFiltered[namesFiltered.Count - 1]);
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
