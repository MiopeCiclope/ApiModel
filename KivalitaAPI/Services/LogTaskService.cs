using System;
using System.Linq;
using System.Threading;
using KivalitaAPI.Data;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Services
{

    public class LogTaskService : Service<LogTask, KivalitaApiContext, LogTaskRepository>
    {

        public LogTaskService(
            KivalitaApiContext context,
            LogTaskRepository baseRepository
        ) : base(context, baseRepository)
        {
        }
    }
}
