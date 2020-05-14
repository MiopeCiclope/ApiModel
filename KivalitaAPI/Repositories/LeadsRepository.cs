
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class LeadsRepository : Repository<Leads, DbContext>
    {
        public LeadsRepository(DbContext context) : base(context) { }
    }
}

