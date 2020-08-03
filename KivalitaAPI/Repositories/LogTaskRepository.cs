
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
   
    }
}
