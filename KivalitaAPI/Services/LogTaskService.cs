using System;
using System.Collections.Generic;
using AutoMapper;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{
    public class LogTaskService : Service<LogTask, KivalitaApiContext, LogTaskRepository>
    {
        IMapper _mapper;
        LogTaskDTORepository _bulkBaseRepository;

        public LogTaskService(
            KivalitaApiContext context,
            LogTaskRepository baseRepository,
            IMapper mapper,
            LogTaskDTORepository bulkBaseRepository
        ) : base(context, baseRepository) {
            _mapper = mapper;
            _bulkBaseRepository = bulkBaseRepository;
        }

        public LogTask GenerateLog(LogTaskEnum logTaskEnum, int leadId, int taskId = 0, int mailId = 0) {
            var logTask = new LogTask
            {
                LeadId = leadId,
                TaskId = taskId,
                MailAnsweredId = mailId,
                CreatedAt = DateTime.Now
            };

            switch (logTaskEnum)
            {
                case LogTaskEnum.LeadAddedToFLow:
                    logTask.Description = "Lead adicionada ao Fluxo";
                    logTask.Type = "add";
                    break;
                case LogTaskEnum.StatusChanged:
                    logTask.Description = "Status alterado";
                    logTask.Type = "update";
                    break;
                case LogTaskEnum.TaskAdded:
                    logTask.Description = "Tarefa adicionada";
                    logTask.Type = "task";
                    break;
                case LogTaskEnum.EmailSent:
                    logTask.Description = "E-mail enviado";
                    logTask.Type = "email";
                    break;
                case LogTaskEnum.EmailRead:
                    logTask.Description = "E-mail lido";
                    logTask.Type = "emailLido";
                    break;
                case LogTaskEnum.EmailAnswered:
                    logTask.Description = "E-mail respondido";
                    logTask.Type = "emailAnswered";
                    break;
            }

            return logTask;
        }

        public void RegisterLog(LogTaskEnum logTaskEnum, int leadId, int taskId = 0, int mailId = 0)
        {
            var logTask = GenerateLog(logTaskEnum, leadId, taskId, mailId);
            baseRepository.Add(logTask);
        }

        public void BulkLog(List<LogTask> logList)
        {
            var bulkList = _mapper.Map<List<LogTaskDatabaseDTO>>(logList);
            _bulkBaseRepository.AddRangeBulkTools(bulkList);
        }
    }
}
