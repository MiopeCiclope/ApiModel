using System;

public class JobScheduleDTO
{
    public JobScheduleDTO(string jobType, string cronExpression, DateTimeOffset? start, int task)
    {
        JobType = Type.GetType(jobType);
        CronExpression = cronExpression;
        JobStart = start;
        TaskId = task;
    }

    public Type JobType { get; }
    public string CronExpression { get; }
    public DateTimeOffset? JobStart { get; set; }
    public int userId { get; set; }
    public int TaskId { get; set; }
}