
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class PostRepository : Repository<Post, DbContext, SieveProcessor>
    {
        public PostRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

