
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KivalitaAPI.Services
{
    public interface IEmailExtractorService
    {

        public Task<string> Run(string firstName, string lastName, string domainCompany);

        public IList<string> GenerateEmails(string firstName, string lastName, string domainCompany);

    }

}
