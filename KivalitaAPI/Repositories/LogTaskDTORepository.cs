
using KivalitaAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class LogTaskDTORepository : Repository<LogTaskDatabaseDTO, DbContext, SieveProcessor>
    {
        public LogTaskDTORepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}

