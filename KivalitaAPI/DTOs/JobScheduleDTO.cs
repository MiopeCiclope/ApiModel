using System;

public class JobScheduleDTO
{
    public JobScheduleDTO(string jobType, string cronExpression, DateTimeOffset start)
    {
        JobType = Type.GetType(jobType);
        CronExpression = cronExpression;
        JobStart = start;
    }

    public Type JobType { get; }
    public string CronExpression { get; }
    public DateTimeOffset? JobStart { get; set; }
    public int userId { get; set; }
}