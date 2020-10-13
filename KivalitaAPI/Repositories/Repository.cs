using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using SqlBulkToolsCore;
using Z.EntityFramework.Extensions;

namespace KivalitaAPI.Repositories {
	public abstract class Repository<TEntity, TContext, TFilterEngine> : IRepository<TEntity>
		where TEntity : class, IEntity
		where TFilterEngine: SieveProcessor
	where TContext : DbContext {
		public readonly TContext context;
		public readonly SieveProcessor filterProcessor;

		private readonly int BatchSize = 200;

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
			var dataTable = GetDataTable(entities);
			ExcuteBulkOperation(dataTable, entities[0].GetType().Name);
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
			if(!entities.Any()) throw new Exception("Update empty list");

			var tableName = typeof(TEntity).GetMethod("GetTableName").Invoke(entities[0], null) as string;
			string csDestination = context.Database.GetDbConnection().ConnectionString;
			var bulk = new BulkOperations(csDestination);

			bulk.Setup<TEntity>(x => x.ForCollection(entities))
				.WithTable(tableName)
				.AddAllColumns()
				.BulkUpdate()
				.SetIdentityColumn(x => x.Id)
				.MatchTargetOn(x => x.Id);

			bulk.CommitTransaction();
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

		private DataTable GetDataTable<TEntity>(IList<TEntity> data)
		{
			var columns = typeof(TEntity).GetMethod("GetDataBaseColumn").Invoke(data[0], null) as string [];
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(TEntity));

			return PushTableData(MapModelToTable(columns, props), columns, data);
		}

		private DataTable MapModelToTable(string[] columns, PropertyDescriptorCollection props)
		{
			DataTable table = new DataTable();
			for (int i = 0; i < props.Count; i++)
			{
				PropertyDescriptor prop = props[i];
				if (columns.Contains(prop.Name))
					table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
			}
			return table;
		}

		private DataTable PushTableData<TEntity>(DataTable table, string[] columns, IList<TEntity> data) {
			object[] values = new object[table.Columns.Count];
			foreach (TEntity item in data)
			{
				for (int i = 0; i < table.Columns.Count; i++)
				{
					var propName = columns[i];
					var value = item.GetType().GetProperty(propName).GetValue(item, null);
					values[i] = value;
				}
				table.Rows.Add(values);
			}
			return table;
		}

		private void ExcuteBulkOperation(DataTable table, string tableName)
		{
			string csDestination = context.Database.GetDbConnection().ConnectionString;
			using (SqlConnection con = new SqlConnection(csDestination))
			{
				using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
				{
					sqlBulkCopy.DestinationTableName = tableName;
					sqlBulkCopy.BatchSize = BatchSize;
					con.Open();
					sqlBulkCopy.WriteToServer(table);
					con.Close();
				}
			}
		}
	}
}
