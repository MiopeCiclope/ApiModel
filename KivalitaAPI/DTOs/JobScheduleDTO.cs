using System;

public class JobScheduleDTO
{
    public JobScheduleDTO(string jobType, string cronExpression)
    {
        JobType = Type.GetType(jobType);
        CronExpression = cronExpression;
    }

    public Type JobType { get; }
    public string CronExpression { get; }
}