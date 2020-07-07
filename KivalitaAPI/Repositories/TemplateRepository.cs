using System.Collections.Generic;
using System.Linq;
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
                .Include(t => t.Category)
                .AsNoTracking()
                .ToList();
        }

        public override Template Get(int id)
        {
            return context.Set<Template>()
                .Where(t => t.Id == id)
                .Include(t => t.Category)
                .SingleOrDefault();
        }

        public override List<Template> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Template>()
                .Include(t => t.Category)
                .AsNoTracking();

            result = this.filterProcessor.Apply(filterQuery, result);
            return result.ToList();
        }
    }
}
