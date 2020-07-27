using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class TaskNoteRepository : Repository<TaskNote, DbContext, SieveProcessor>
    {
        public TaskNoteRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }
    }
}
