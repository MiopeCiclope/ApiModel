using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace KivalitaAPI.Repositories {
	public class FilterRepository : Repository<Filter, DbContext, SieveProcessor> {
		public FilterRepository (DbContext context, SieveProcessor filterProcessor) : base (context, filterProcessor) { }

		public override Filter Add (Filter entity) {
			var filterSearch = this.GetBy (storedFilter => storedFilter.Name == entity.Name);
			var filterExists = filterSearch.Any () ? filterSearch.First () : null;
			if (filterExists != null) {
				return null;
			} else {
				entity.SieveFilter = entity.GetSieveFilter();
				var filter = base.Add (entity);
				return filter;
			}
		}

		public override Filter Update(Filter entity)
		{
			entity.SieveFilter = entity.GetSieveFilter();
			return base.Update(entity);
		}


		public override List<Filter> GetBy (Func<Filter, bool> condition) {
			var filters = base.GetBy (condition);
			filters = filters.Select (filter => {
				return filter;
			}).ToList ();
			return filters;
		}
	}
}
