
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class PostRepository : Repository<Post, DbContext>
    {
        public PostRepository(DbContext context) : base(context) { }
    }
}

