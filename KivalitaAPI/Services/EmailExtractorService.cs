using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KivalitaAPI.Services
{
    public class EmailExtractorService : IEmailExtractorService
    {

        private RequestService request;
        private readonly string apiUrl = "https://www.baselimpa.com/controllers/ValidateMail.ashx";

        public EmailExtractorService(RequestService _requestService)
        {
            this.request = _requestService;
        }

        async public Task<string> Run(string firstName, string lastName, List<string> domainsCompany)
        {
            var emails = GenerateEmails(firstName, lastName, domainsCompany);

            foreach (string email in emails)
            {
                IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("mail", email)
                };

                try
                {
                    var response = await request.PostFormAsync(apiUrl, queries);

                    if (response["Status"] == "2")
                    {
                        return email;
                    }
                }
                catch
                {
                    Console.WriteLine("Email Timeout!");
                }
            }

            return null;
        }

        public IList<string> GenerateEmails(string firstName, string lastName, List<string> domainsCompany)
        {
            var emails = new List<string> { };
            foreach (string domain in domainsCompany)
            {
                emails.Add($"{firstName}.{lastName}@{domain}");
                emails.Add($"{firstName}{lastName}@{domain}");
                emails.Add($"{firstName}@{domain}");
                emails.Add($"{((firstName.Length > 0) ? firstName[0] : (char?)null)}{lastName}@{domain}");
            }

            return emails;
        }

    }
}
