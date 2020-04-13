using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Data
{
    public class KivalitaApiContext : DbContext
    {
        public KivalitaApiContext(DbContextOptions<KivalitaApiContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
