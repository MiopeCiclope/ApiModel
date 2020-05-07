
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using System.Linq;
using System.Collections.Generic;
using System;

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

        public override User Get(int id)
        {
            var user = base.Get(id);
            user.Password = "";
            return user;
        }

        public override List<User> GetAll()
        {
            var users = base.GetAll();
            users = users.Select(user =>
            {
                user.Password = "";
                return user;
            }).ToList();
            return users;
        }

        public override User Add(User entity)
        {
            var user = base.Add(entity);
            user.Password = "";
            return user;
        }

        public override User Update(User entity)
        {
            if (entity.Password is null)
            {
                var currentUser = base.Get(entity.Id);
                entity.Password = currentUser.Password;
            }

            var user = base.Update(entity);
            user.Password = "";
            return user;
        }

        public override List<User> GetBy(Func<User, bool> condition)
        {
            var users = base.GetBy(condition);
            users = users.Select(user =>
            {
                user.Password = "";
                return user;
            }).ToList();
            return users;
        }
    }
}

