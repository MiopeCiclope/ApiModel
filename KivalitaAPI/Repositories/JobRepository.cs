
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class JobRepository : Repository<Job, DbContext, SieveProcessor>
    {
        public JobRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

