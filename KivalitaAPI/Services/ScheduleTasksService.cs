using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;

namespace KivalitaAPI.Services
{

    public class ScheduleTasksService
    {
        LeadsRepository _leadsRepository;
        FlowTaskRepository _flowTaskRepository;
        public readonly IJobScheduler _scheduler;

        public ScheduleTasksService (
            FlowTaskRepository flowTaskRepository,
            LeadsRepository leadsRepository,
            IJobScheduler scheduler
        )
        { 
            _flowTaskRepository = flowTaskRepository;
            _leadsRepository = leadsRepository;
            _scheduler = scheduler;
        }

        public void Execute(Flow flow, List<Leads> leads)
        {
            CancellationToken cancellationToken = new CancellationToken();

            var leadGroup = SplitLeadGroupByDay(leads, flow);

            var daysToAdd = 0;
            foreach (var action in flow.FlowAction)
            {
                foreach (var leadList in leadGroup)
                {

                    foreach (var lead in leadList)
                    {

                        lead.Status = LeadStatusEnum.Flow;
                        lead.FlowId = flow.Id;
                        _leadsRepository.Update(lead);

                        var taskPayload = new FlowTask
                        {
                            Status = "pending",
                            LeadId = lead.Id,
                            FlowActionId = action.Id,
                            CreatedAt = DateTime.Now,
                            CreatedBy = flow.CreatedBy,
                            UpdatedAt = DateTime.Now
                        };

                        if (flow.FlowAction.First() == action)
                        {
                            taskPayload.ScheduledTo = DateTime.Now.AddDays(action.afterDays + daysToAdd);
                        }

                        var task = _flowTaskRepository.Add(taskPayload);

                        if (task.ScheduledTo.HasValue)
                        {
                            DateTimeOffset dateTimeOffset = new DateTimeOffset((DateTime)task.ScheduledTo);
                            var job = new JobScheduleDTO("TaskJob", "0/2 * * * * ?", dateTimeOffset, task.Id)
                            {
                                userId = flow.CreatedBy
                            };
                            _scheduler.ScheduleJob(cancellationToken, job);
                        }

                    }
                    daysToAdd += 1;
                }
            }
        }

        private List<List<Leads>> SplitLeadGroupByDay(List<Leads> leads, Flow flow)
        {
            return flow.actionForAllLeads
                ? SplitList(leads, flow.leadGroupSize)
                : SplitList(leads, 1);
        }

        private List<List<T>> SplitList<T>(List<T> list, int number)
        {
            return list
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / number)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
