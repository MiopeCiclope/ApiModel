using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Interfaces;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

public class SchedulerService : IJobScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobFactory _jobFactory;
    private readonly IEnumerable<JobScheduleDTO> _jobSchedules;

    public SchedulerService(
        ISchedulerFactory schedulerFactory,
        IJobFactory jobFactory,
        IEnumerable<JobScheduleDTO> jobSchedules)
    {
        _schedulerFactory = schedulerFactory;
        _jobSchedules = jobSchedules;
        _jobFactory = jobFactory;
    }
    public IScheduler Scheduler { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;

        foreach (var jobSchedule in _jobSchedules)
        {
            var job = CreateJob(jobSchedule);
            var trigger = CreateTrigger(jobSchedule);

            await Scheduler.ScheduleJob(job, trigger, cancellationToken);
        }

        await Scheduler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await Scheduler?.Shutdown(cancellationToken);
    }

    private static IJobDetail CreateJob(JobScheduleDTO schedule)
    {
        var jobType = schedule.JobType;
        var jobIdentity = jobType.FullName + DateTime.Now.Ticks.ToString();

        return JobBuilder
            .Create(jobType)
            .WithIdentity(jobIdentity)
            .WithDescription(jobType.Name)
            .Build();
    }

    private static ITrigger CreateTrigger(JobScheduleDTO schedule)
    {
        var triggerIdentity = $"{schedule.JobType.FullName}{DateTime.Now.Ticks.ToString()}.trigger";
        return TriggerBuilder
            .Create()
            .WithIdentity(triggerIdentity)
            .WithCronSchedule(schedule.CronExpression)
            .WithDescription(schedule.CronExpression)
            .Build();
    }

    public async Task<DateTimeOffset> ScheduleJob(CancellationToken cancellationToken, JobScheduleDTO newJob)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;

        var job = CreateJob(newJob);
        var trigger = CreateTrigger(newJob);

        return await Scheduler.ScheduleJob(job, trigger, cancellationToken);
    }
}