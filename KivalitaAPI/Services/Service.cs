using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Interfaces;
using System.Collections.Generic;

namespace KivalitaAPI.Services
{
    public abstract class Service<TEntity, TContext, TRepository> : IService<TEntity>
        where TEntity : class, IEntity
        where TRepository : class, IRepository<TEntity>
        where TContext : DbContext
    {
        public readonly TContext context;
        public readonly TRepository baseRepository;

        public Service(TContext context, TRepository repository)
        {
            this.context = context;
            this.baseRepository = repository;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return baseRepository.Add(entity);
        }

        public virtual TEntity[] AddRange(TEntity[] entities)
        {
            return baseRepository.AddRange(entities);
        }

        public virtual TEntity Delete(int id)
        {
            return baseRepository.Delete(id);
        }

        public virtual TEntity Get(int id)
        {
            return baseRepository.Get(id);
        }

        public virtual List<TEntity> GetAll()
        {
            return baseRepository.GetAll();
        }

        public virtual TEntity Update(TEntity entity)
        {
            return baseRepository.Update(entity);
        }
    }
}
