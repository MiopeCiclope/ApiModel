using System;
using System.Threading.Tasks;
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

            string firstName;
            string lastName;
            string domain;

            domain = GetDomain(lead.CompanySite);
            (firstName, lastName) = GetFirstAndLastName(lead.name);

            string email = await emailExtractorService.Run(firstName, lastName, domain);

            if (email != null)
            {
                lead.email = email;
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
            var names = fullName.ToLower().Split(' ');
            switch (names.Length)
            {
                case 0:
                    return ("", "");
                case 1:
                    return (names[0], "");
                case 2:
                    return (names[0], names[1]);
                default:
                    return (names[0], names[names.Length - 1]);
            }
        }
    }
}
