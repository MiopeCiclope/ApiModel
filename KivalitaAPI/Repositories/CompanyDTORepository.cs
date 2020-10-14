
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class CompanyDTORepository : Repository<CompanyDatabaseDTO, DbContext, SieveProcessor>
    {
        public CompanyDTORepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

