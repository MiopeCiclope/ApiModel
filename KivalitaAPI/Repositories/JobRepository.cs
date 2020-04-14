
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class JobRepository : Repository<Job, DbContext>
    {
        public JobRepository(DbContext context) : base(context) { }
    }
}

