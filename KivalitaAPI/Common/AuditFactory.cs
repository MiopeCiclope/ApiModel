using AutoMapper;
using KivalitaAPI.AuditModels;
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
                    var userAudit = _mapper.Map<UserHistory>(user);
                    return addAuditComplement(userAudit, action, responsable);
                case Token token:
                    var tokenAudit = _mapper.Map<TokenHistory>(token);
                    return addAuditComplement(tokenAudit, action, responsable);
                case Post post:
                    var postAudit = _mapper.Map<PostHistory>(post);
                    return addAuditComplement(postAudit, action, responsable);
                case Image image:
                    var imageAudit = _mapper.Map<ImageHistory>(image);
                    return addAuditComplement(imageAudit, action, responsable);
                case Job job:
                    var jobAudit = _mapper.Map<JobHistory>(job);
                    return addAuditComplement(jobAudit, action, responsable);
                case Leads lead:
                    var leadAudit = _mapper.Map<LeadsHistory>(lead);
                    return addAuditComplement(leadAudit, action, responsable);
                case Company company:
                    var companyAudit = _mapper.Map<CompanyHistory>(company);
                    return addAuditComplement(companyAudit, action, responsable);
                default:
                    var defaultAudit = new UserHistory();
                    defaultAudit.FirstName = "default";
                    return addAuditComplement(defaultAudit, action, responsable);
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
            data.Date = DateTime.UtcNow;
            data.Responsable = responsable;
            return data;
        }
    }
}
