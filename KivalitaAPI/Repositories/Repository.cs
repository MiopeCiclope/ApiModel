using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public abstract class Repository<TEntity, TContext> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TContext : DbContext
    {
        public readonly TContext context;
        public Repository(TContext context)
        {
            this.context = context;
        }
        public virtual TEntity Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public virtual TEntity Delete(int id)
        {
            var entity = context.Set<TEntity>().Find(id);
            if (entity == null)
            {
                return entity;
            }

            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();

            return entity;
        }

        public virtual TEntity Get(int id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public virtual List<TEntity> GetAll()
        {
            return context.Set<TEntity>().ToList();
        }

        public virtual List<TEntity> GetBy(Func<TEntity, bool> condition)
        {
            return context.Set<TEntity>().Where(condition).ToList();
        }

        public virtual TEntity Update(TEntity entity)
        {
            var local = context.Set<TEntity>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(entity).State = EntityState.Modified;
            context.SaveChanges();
            return entity;
        }

        public virtual void ReverseUpdateState(TEntity entity)
        {
            var local = context.Set<TEntity>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            if (local != null)
                context.Entry(local).State = EntityState.Detached;
        }
    }
}
