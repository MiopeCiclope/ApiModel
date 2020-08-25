
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class TagRepository : Repository<Tag, DbContext, SieveProcessor>
    {
        public TagRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

