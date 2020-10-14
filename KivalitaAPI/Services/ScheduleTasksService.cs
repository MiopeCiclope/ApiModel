using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using KivalitaAPI.Common;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Quartz;
using Sieve.Models;

namespace KivalitaAPI.Services
{

    public class ScheduleTasksService
    {
        LeadsRepository _leadsRepository;
        FlowTaskRepository _flowTaskRepository;
        LogTaskService _logTaskService;
        public readonly IJobScheduler _scheduler;
        IMapper _mapper;
        LeadsDTORepository _leadsDTORepository;

        public ScheduleTasksService (
            FlowTaskRepository flowTaskRepository,
            LeadsRepository leadsRepository,
            LogTaskService logTaskService,
            IJobScheduler scheduler, 
            IMapper mapper, 
            LeadsDTORepository leadsDTORepository
        )
        { 
            _flowTaskRepository = flowTaskRepository;
            _leadsRepository = leadsRepository;
            _scheduler = scheduler;
            _logTaskService = logTaskService;
            _mapper = mapper;
            _leadsDTORepository = leadsDTORepository;
        }

        public void Execute(Flow flow, List<Leads> leads)
        {
            CancellationToken cancellationToken = new CancellationToken();

            var leadGroup = SplitLeadGroupByDay(leads, flow);
            List<Leads> leadListToUpdate = new List<Leads>();
            List<FlowTask> taskList = new List<FlowTask>();

            var daysAllowedToSchedule = flow.DaysOfTheWeek.Split(',').Select(Int32.Parse).ToList();
            var businessDate = DateUtils.GetDateSheduleValid(DateTime.Now, daysAllowedToSchedule);

            var daysToAdd = 0;
            foreach (var action in flow.FlowAction)
            {
                var date = businessDate.AddDays(action.afterDays);
                foreach (var leadList in leadGroup)
                {
                    date = date.AddDays(daysToAdd);

                    foreach (var lead in leadList)
                    {
                        if (!lead.FlowId.HasValue || lead.FlowId != flow.Id)
                        {
                            lead.FlowId = flow.Id;
                            leadListToUpdate.Add(lead);
                            _logTaskService.RegisterLog(LogTaskEnum.LeadAddedToFLow, lead.Id);
                        }

                        if (lead.Status != LeadStatusEnum.Flow)
                        {
                            lead.Status = LeadStatusEnum.Flow;
                            leadListToUpdate.Add(lead);
                            _logTaskService.RegisterLog(LogTaskEnum.StatusChanged, lead.Id);
                        }
                        
                        var task = new FlowTask
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
                            date = DateUtils.GetDateSheduleValid(date, daysAllowedToSchedule);
                            task.ScheduledTo = date;
                        }

                        taskList.Add(task);
                        _logTaskService.RegisterLog(LogTaskEnum.TaskAdded, lead.Id, task.Id);
                    }
                    daysToAdd = 1;
                }
            }

            if (leadListToUpdate.Any())
            {
                var bulkListLeads = _mapper.Map<List<LeadDatabaseDTO>>(leadListToUpdate.Distinct().ToList());
                _leadsDTORepository.UpdateRange(bulkListLeads);
            }
            if (taskList.Any())
                _flowTaskRepository.AddRange(taskList);
        }

        public void RemoveRange(List<Leads> leads)
        {
            foreach( var lead in leads ) {
                this.Remove(lead);
            }
        }

        public void Remove(Leads lead)
        {
            var tasksPending = _flowTaskRepository.GetPendingByLead(lead.Id);
            foreach (var task in tasksPending)
            {
                var job = new JobKey($"TaskJob_{task.Id}", "DEFAULT");
                _scheduler.DeleteJob(job);
            }

            _flowTaskRepository.DeleteRange(tasksPending);

            lead.FlowId = null;
            _leadsRepository.Update(lead);
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
