
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KivalitaAPI.Services
{
    public interface IEmailExtractorService
    {

        public Task<string> Run(string firstName, string lastName, List<string> domainsCompany);

        public IList<string> GenerateEmails(string firstName, string lastName, List<string> domainsCompany);

    }

}
