
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using Sieve.Services;
using KivalitaAPI.DTOs;
using KivalitaAPI.Enum;

namespace KivalitaAPI.Repositories
{
    public class UserRepository : Repository<User, DbContext, SieveProcessor>
    {
        public UserRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public User GetByLoginData(User user)
        {
            var userSearch = this.GetBy(
                storedUser => storedUser.Email == user.Email &&
                storedUser.Password == user.Password &&
                storedUser.Active == true
            );
            var userLogged = userSearch.Any() ? userSearch.First() : null;
            return userLogged;
        }

        public override User Get(int id)
        {
            var user = context.Set<User>()
                .Include(u => u.Company)
                .SingleOrDefault(u => u.Id == id);

            return removePassword(user);
        }

        public override List<User> GetAll()
        {
            var users = context.Set<User>()
                .Include(u => u.Company)
                .ToList();

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

        public List<TaskDTO> GetTaskData()
        {
            var query = $@"SELECT DISTINCT f.owner as UserId, 
                                            ft.leadid as LeadId, 
                                            L.email as Email, 
                                            ft.id as TaskId
                            FROM   flowtask ft 
                                   INNER JOIN(SELECT id, 
                                                     flowid 
                                              FROM   flowaction 
                                              WHERE  type = 'email') fa 
                                           ON fa.id = ft.flowactionid 
                                   INNER JOIN(SELECT id, 
                                                     owner 
                                              FROM   flow) f 
                                           ON f.id = fa.flowid 
                                   INNER JOIN (SELECT id, 
                                                      email 
                                               FROM   leads
                                               WHERE  Status = {(int) LeadStatusEnum.Flow}) L 
                                           ON L.id = FT.leadid 
                            WHERE  ft.status = 'finished'
                                    and f.owner in (select UseriD FROM MicrosoftToken WHERE AccessToken IS NOT NULL)";

            return context.Set<TaskDTO>().FromSqlRaw(query).ToList();
        }
    }
}

