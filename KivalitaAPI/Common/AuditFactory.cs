using AutoMapper;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KivalitaAPI.Common
{
    public class AuditFactory
    {
        private readonly IMapper _mapper;

        public AuditFactory(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IAuditTable GetAuditObject(IEntity entity, EntityState action, int responsable)
        {
            switch (entity)
            {
                case User user:
                    var auditData = _mapper.Map<UserHistory>(user);
                    return addAuditComplement(auditData, action, responsable);
                default:
                    auditData = new UserHistory();
                    auditData.FirstName = "default";
                    return addAuditComplement(auditData, action, responsable);
            }
        }

        private static IAuditTable addAuditComplement(IAuditTable data, EntityState action, int responsable)
        {
            switch (action)
            {
                case EntityState.Added:
                    data.Action = ActionEnum.Insert;
                    break;
                case EntityState.Modified:
                    data.Action = ActionEnum.Update;
                    break;
                case EntityState.Deleted:
                    data.Action = ActionEnum.Delete;
                    break;
            }
            data.Date = DateTime.Now;
            data.Responsable = responsable;
            return data;
        }
    }
}
