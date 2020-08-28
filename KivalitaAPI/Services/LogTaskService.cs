using System;
using KivalitaAPI.Data;
using KivalitaAPI.Enum;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{
    public class LogTaskService : Service<LogTask, KivalitaApiContext, LogTaskRepository>
    {

        public LogTaskService(
            KivalitaApiContext context,
            LogTaskRepository baseRepository
        ) : base(context, baseRepository) { }

        public void RegisterLog(LogTaskEnum logTaskEnum, int leadId, int taskId = 0)
        {
            var logTask = new LogTask
            {
                LeadId = leadId,
                TaskId = taskId,
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
                    logTask.Type = "email";
                    break;
            }

            baseRepository.Add(logTask);
        }
    }
}
