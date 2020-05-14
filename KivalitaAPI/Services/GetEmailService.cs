using System;
using System.Threading.Tasks;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{
    public class GetEmailService
    {
        LeadsRepository leadsRepository;
        IEmailExtractorService emailExtractorService;

        public GetEmailService(LeadsRepository leadsRepository, IEmailExtractorService emailExtractorService)
        {
            this.leadsRepository = leadsRepository;
            this.emailExtractorService = emailExtractorService;
        }

        public async Task FromLeadId(int leadId) {
            //EmailExtractorQueue queue = new EmailExtractorQueue();
            //queue.Enqueue(() => GuessEmailLeadAsync(leadId));
            await GuessEmailLeadAsync(leadId);
            
        }

        private async Task GuessEmailLeadAsync(int leadId)
        {
            Console.WriteLine("Init Task ----------------");
            Leads lead = leadsRepository.Get(leadId);
            string firstName;
            string lastName;
            string domain;

            Console.WriteLine(lead.name);

            domain = GetDomain(lead.CompanySite);
            (firstName, lastName) = GetFirstAndLastName(lead.name);

            string email = await emailExtractorService.Run(firstName, lastName, domain);

            //string email = "douglas.bosco@kivalita.com";
            Console.WriteLine(email);

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
