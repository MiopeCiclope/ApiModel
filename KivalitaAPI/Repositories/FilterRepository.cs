using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Repositories {
	public class FilterRepository : Repository<Filter, DbContext> {
		public FilterRepository (DbContext context) : base (context) { }

		public override Filter Add (Filter entity) {
			var filterSearch = this.GetBy (storedFilter => storedFilter.Name == entity.Name);
			var filterExists = filterSearch.Any () ? filterSearch.First () : null;
			if (filterExists != null) {
				return null;
			} else {
				Console.WriteLine (entity);
				var filter = base.Add (entity);
				return filter;
			}

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
