using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using KivalitaAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Z.EntityFramework.Extensions;

namespace KivalitaAPI.Repositories {
	public abstract class Repository<TEntity, TContext, TFilterEngine> : IRepository<TEntity>
		where TEntity : class, IEntity
		where TFilterEngine: SieveProcessor
	where TContext : DbContext {
		public readonly TContext context;
		public readonly SieveProcessor filterProcessor;

		public Repository (TContext context, SieveProcessor _filterProcessor) {
			this.context = context;
			this.filterProcessor = _filterProcessor;
		}
		public virtual TEntity Add (TEntity entity) {
			context.Set<TEntity> ().Add (entity);
			context.SaveChanges ();
			return entity;
		}

		public virtual List<TEntity> AddRange (List<TEntity> entities) {
			context.Set<TEntity>().BulkInsert(entities, options => options.BatchSize = 200);
			return entities;
		}

		public virtual TEntity Delete (int id, int userId) {
			var entity = context.Set<TEntity> ().Find (id);
			entity.UpdatedBy = userId;
			if (entity == null) {
				return entity;
			}

			context.Set<TEntity> ().Remove (entity);
			context.SaveChanges ();

			return entity;
		}

		public virtual TEntity Get (int id) {
			return context.Set<TEntity> ().Find (id);
		}

		public virtual ValueTask<TEntity> GetAsync (int id) {
			return context.Set<TEntity> ().FindAsync (id);
		}

		public virtual List<TEntity> GetAll () {
			return context.Set<TEntity> ().ToList ();
		}

		public virtual List<TEntity> GetBy (Func<TEntity, bool> condition) {
			return context.Set<TEntity> ().Where (condition).ToList ();
		}

		public virtual TEntity Update (TEntity entity) {
			entity.UpdatedAt = DateTime.UtcNow;
			var local = context.Set<TEntity> ()
				.Local
				.FirstOrDefault (entry => entry.Id.Equals (entity.Id));
			if (local != null) {
				context.Entry (local).State = EntityState.Detached;
			}
			context.Entry(entity).State = EntityState.Modified;
			context.SaveChanges ();
			return entity;
		}

		public virtual List<TEntity> UpdateRange(List<TEntity> entities)
		{
			context.Set<TEntity>().BulkUpdate(entities, options =>
			{
				options.IgnoreOnUpdateExpression = entity => new
				{
					entity.CreatedAt,
					entity.CreatedBy
				};
			});
			return entities;
		}

		public virtual void ReverseUpdateState (TEntity entity) {
			var local = context.Set<TEntity> ()
				.Local
				.FirstOrDefault (entry => entry.Id.Equals (entity.Id));

			if (local != null)
				context.Entry (local).State = EntityState.Detached;
		}

		public virtual QueryResult<TEntity> GetAll_v2(SieveModel filterQuery)
		{
			var result = context.Set<TEntity>().AsNoTracking();
			var total = result.Count();
			result = this.filterProcessor.Apply(filterQuery, result);

			return new QueryResult<TEntity>
			{
				Items = result.ToList(),
				TotalItems = total,
			};
		}

		public virtual List<TEntity> DeleteRange(List<TEntity> entities)
		{
			context.Set<TEntity>().RemoveRange(entities);
			context.SaveChanges();
			return entities;
		}

		public List<TEntity> GetListByQuery(string query)
		{
			return context.Set<TEntity>().FromSqlRaw(query).ToList();
		}

		public TEntity GetByQuery(string query)
		{
			return context.Set<TEntity>().FromSqlRaw(query)?.First() ?? null;
		}
	}
}
