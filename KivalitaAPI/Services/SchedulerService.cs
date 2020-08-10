using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KivalitaAPI.Interfaces;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl.Matchers;
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
        var jobIdentity = $"{jobType.FullName}_{schedule.TaskId}";

        return JobBuilder
            .Create(jobType)
            .WithIdentity(jobIdentity)
            .WithDescription(jobType.Name)
            .Build();
    }

    private static ITrigger CreateTrigger(JobScheduleDTO schedule)
    {
        var triggerIdentity = $"{schedule.JobType.FullName}{DateTime.Now.Ticks.ToString()}.trigger";
        if (schedule.JobStart == null)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity(triggerIdentity)
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
        else
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerIdentity)
                .StartAt(schedule.JobStart.GetValueOrDefault(DateTimeOffset.UtcNow))
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }

    public async Task<DateTimeOffset> ScheduleJob(CancellationToken cancellationToken, JobScheduleDTO newJob)
    {
        Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
        Scheduler.JobFactory = _jobFactory;

        var job = CreateJob(newJob);
        job.JobDataMap["userId"] = newJob.userId;
        job.JobDataMap["taskId"] = newJob.TaskId;
        var trigger = CreateTrigger(newJob);

        return await Scheduler.ScheduleJob(job, trigger, cancellationToken);
    }

    public List<JobScheduleDTO> GetScheduledJobs()
    {
        var jobGroups = Scheduler.GetJobGroupNames().Result;
        if (!jobGroups.Any()) return null;

        List<JobScheduleDTO> scheludedJobs = new List<JobScheduleDTO>();
        foreach (string group in jobGroups)
        {
            var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
            var jobKeys = Scheduler.GetJobKeys(groupMatcher).Result;
            foreach (var jobKey in jobKeys)
            {
                var detail = Scheduler.GetJobDetail(jobKey).Result;
                var triggers = Scheduler.GetTriggersOfJob(jobKey).Result;
                foreach (ITrigger trigger in triggers)
                {
                    DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                    if (nextFireTime.HasValue)
                    {
                        var nameSplit = jobKey.Name.Split("_");
                        scheludedJobs.Add(new JobScheduleDTO(nameSplit[0], "", nextFireTime.Value.LocalDateTime, int.Parse(nameSplit[1])));
                    }
                }
            }
        }
        return scheludedJobs;
    }

    public async Task DeleteJob(JobKey jobKey)
    {
        if (await Scheduler.CheckExists(jobKey))
        {
            await Scheduler.DeleteJob(jobKey);
        }
    }
}