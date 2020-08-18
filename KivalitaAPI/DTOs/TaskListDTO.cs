using KivalitaAPI.Models;
using System.Collections.Generic;

namespace KivalitaAPI.DTOs
{
    public class TaskListDTO
    {
        public List<FlowTask> futureTasks { get; set; }
        public List<FlowTask> overdueTasks { get; set; }
        public List<FlowTask> todayTasks { get; set; }
    }
}
