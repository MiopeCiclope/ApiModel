
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
            return removePassword(user);
        }

        public override List<User> GetAll()
        {
            var users = base.GetAll();
            users = users.Select(user =>
            {
                return removePassword(user);
            }).ToList();
            return users;
        }

        public override User Add(User entity)
        {
            var user = base.Add(entity);
            return removePassword(user);
        }

        public override User Update(User entity)
        {
            if (entity.Password is null)
            {
                var currentUser = base.Get(entity.Id);
                entity.Password = currentUser.Password;
            }

            var user = base.Update(entity);
            return removePassword(user);
        }

        public override List<User> GetBy(Func<User, bool> condition)
        {
            var users = base.GetBy(condition);
            users = users.Select(user =>
            {
                return removePassword(user);
            }).ToList();
            return users;
        }

        private User removePassword(User user)
        {
            user.Password = "";
            base.ReverseUpdateState(user);
            return user;
        }
    }
}

