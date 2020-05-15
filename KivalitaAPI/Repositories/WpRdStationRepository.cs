
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class WpRdStationRepository : Repository<WpRdStation, DbContext>
    {
        public WpRdStationRepository(DbContext context) : base(context) { }
    }
}

