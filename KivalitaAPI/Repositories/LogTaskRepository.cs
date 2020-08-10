
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;
using System.Collections.Generic;
using KivalitaAPI.Common;
using Sieve.Models;
using Microsoft.Linq.Translations;
using System;

namespace KivalitaAPI.Repositories
{
    public class LogTaskRepository : Repository<LogTask, DbContext, SieveProcessor>
    {
        public LogTaskRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override List<LogTask> GetAll()
        {
            return context.Set<LogTask>()
                .Include(f => f.Leads)
                    .ThenInclude(l => l.Company)
                .WithTranslations()
                .AsNoTracking()
                .ToList();
        }

        public override LogTask Get(int id)
        {
            return context.Set<LogTask>()
                .Where(f => f.Id == id)
                .Include(f => f.Leads)
                    .ThenInclude(l => l.Company)
                .WithTranslations()
                .AsNoTracking()
                .SingleOrDefault();
        }
    }
}
