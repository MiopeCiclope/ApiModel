using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Interfaces;
using System.Collections.Generic;
using Sieve.Models;
using KivalitaAPI.Common;

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

        public virtual List<TEntity> AddRange(List<TEntity> entities)
        {
            return baseRepository.AddRange(entities);
        }

        public virtual TEntity Delete(int id, int userId)
        {
            return baseRepository.Delete(id, userId);
        }

        public List<TEntity> DeleteRange(List<TEntity> entities)
        {
            return baseRepository.DeleteRange(entities);
        }

        public virtual TEntity Get(int id)
        {
            return baseRepository.Get(id);
        }

        public virtual List<TEntity> GetAll()
        {
            return baseRepository.GetAll();
        }

        public virtual QueryResult<TEntity> GetAll_v2(SieveModel filterQuery)
        {
            return baseRepository.GetAll_v2(filterQuery);
        }

        public virtual TEntity Update(TEntity entity)
        {
            return baseRepository.Update(entity);
        }
    }
}
