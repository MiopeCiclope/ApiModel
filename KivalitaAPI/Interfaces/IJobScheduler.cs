using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace KivalitaAPI.Interfaces
{
    public interface IJobScheduler : IHostedService
    {
        Task<DateTimeOffset> ScheduleJob(CancellationToken cancellationToken, JobScheduleDTO newJob);
    }
}
