using System;
using System.Data.Entity;
using System.Linq;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class TaskNoteService : Service<TaskNote, KivalitaApiContext, TaskNoteRepository>
    {
        FlowTaskRepository _flowTaskRepository;

        public TaskNoteService(
            KivalitaApiContext context,
            TaskNoteRepository baseRepository,
            FlowTaskRepository flowTaskRepository
        ) : base(context, baseRepository)
        {
            _flowTaskRepository = flowTaskRepository;
        }

        public override TaskNote Add(TaskNote entity)
        {
            var existingTask = _flowTaskRepository
                .GetBy(f => f.Id == entity.FlowTaskId)
                .SingleOrDefault();

            if (existingTask == null)
                throw new Exception("Tarefa não encontrada");

            entity.FlowTaskId = existingTask.Id;

            return base.Add(entity);
        }
    }
}
