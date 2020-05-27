﻿using System;
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

        async public Task<string> Run(string firstName, string lastName, string domainCompany)
        {
            var emails = GenerateEmails(firstName, lastName, domainCompany);

            foreach (string email in emails)
            {

                IEnumerable<KeyValuePair<string, string>> queries = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("mail", email)
                };

                var response = await request.PostFormAsync(apiUrl, queries);

                if (response["Status"] == "2")
                {
                    return email;
                }
            }

            return null;

        }

        public IList<string> GenerateEmails(string firstName, string lastName, string domainCompany)
        {
            var emails = new List<string>
            {
                $"{firstName}.{lastName}@{domainCompany}",
                $"{firstName}{lastName}@{domainCompany}",
                $"{firstName}{((lastName.Length > 0) ? lastName[0] : (char?)null)}@{domainCompany}",
                $"{((firstName.Length > 0) ? firstName[0] : (char?)null)}{lastName}@{domainCompany}",
                $"{firstName}@{domainCompany}",
            };

            return emails;
        }

    }
}