
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class UserRepository : Repository<User, DbContext>
    {
        public UserRepository(DbContext context) : base(context) { }

        public User GetByLoginData(User user)
        {
            var userSearch = this.GetBy(storedUser => storedUser.Email == user.Email && storedUser.Password == user.Password);
            var userLogged = userSearch.Any() ? userSearch.First() : null;
            return userLogged;
        }
    }
}

