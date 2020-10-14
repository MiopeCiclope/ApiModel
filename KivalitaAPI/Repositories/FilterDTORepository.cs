
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class FilterDTORepository : Repository<FilterDatabaseDTO, DbContext, SieveProcessor>
    {
        public FilterDTORepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

