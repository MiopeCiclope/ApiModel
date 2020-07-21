
using Microsoft.EntityFrameworkCore;
using KivalitaAPI.Models;
using Sieve.Services;
using System.Linq;

namespace KivalitaAPI.Repositories
{
    public class FlowTaskRepository : Repository<FlowTask, DbContext, SieveProcessor>
    {
        public FlowTaskRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) { }

        public override FlowTask Get(int id)
        {
            return context.Set<FlowTask>()
                .Where(f => f.Id == id)
                .Include(f => f.FlowAction)
                .AsNoTracking()
                .SingleOrDefault();
        }

        public FlowTask GetNextTask(FlowTask currentTask)
        {
            return context.Set<FlowTask>()
                .Where(
                    f => f.Status == "pending" &&
                    f.LeadId == currentTask.LeadId &&
                    f.Id != currentTask.Id
                )
                .Include(f => f.FlowAction)
                .FirstOrDefault();
        }
    }
}
