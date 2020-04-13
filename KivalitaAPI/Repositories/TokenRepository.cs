using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;

namespace KivalitaAPI.Repositories
{
    public class TokenRepository : Repository<Token, DbContext>
    {
        public TokenRepository(DbContext context) : base(context) { }
    }
}

