using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Common;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories {
	public class TemplateRepository: Repository<Template, DbContext, SieveProcessor> {
		public TemplateRepository(DbContext context, SieveProcessor templateProcessor) : base (context, templateProcessor) { }

        public override List<Template> GetAll()
        {
            return context.Set<Template>()
                .AsNoTracking()
                .ToList();
        }

        public List<Template> GetAllAsNoTracking()
        {
            return context.Set<Template>()
                .AsNoTracking()
                .ToList();
        }

        public override Template Get(int id)
        {
            return context.Set<Template>()
                .Where(t => t.Id == id)
                .SingleOrDefault();
        }

        public override QueryResult<Template> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Template>()
                .AsNoTracking();

            var total = result.Count();

            result = this.filterProcessor.Apply(filterQuery, result);
            return new QueryResult<Template>
            {
                Items = result.ToList(),
                TotalItems = total,
            };
        }
    }
}
