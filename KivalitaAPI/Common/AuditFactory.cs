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
                case Category category:
                    var categoryAudit = _mapper.Map<CategoryHistory>(category);
                    return addAuditComplement(categoryAudit, action, responsable);
                case Template template:
                    var templateAudit = _mapper.Map<TemplateHistory>(template);
                    return addAuditComplement(templateAudit, action, responsable);
                case Flow flow:
                    var flowAudit = _mapper.Map<FlowHistory>(flow);
                    return addAuditComplement(flowAudit, action, responsable);
                case FlowAction flowAction:
                    var flowActionAudit = _mapper.Map<FlowActionHistory>(flowAction);
                    return addAuditComplement(flowActionAudit, action, responsable);
                case FlowTask flowTask:
                    var flowTaskAudit = _mapper.Map<FlowTaskHistory>(flowTask);
                    return addAuditComplement(flowTaskAudit, action, responsable);
                case TaskNote taskNote:
                    var taskNoteAudit = _mapper.Map<TaskNoteHistory>(taskNote);
                    return addAuditComplement(taskNoteAudit, action, responsable);
                case MailSignature mailSignature:
                    var mailSignatureAudit = _mapper.Map<MailSignatureHistory>(mailSignature);
                    return addAuditComplement(mailSignatureAudit, action, responsable);
                case MailServer mailServer:
                    var mailServerAudit = _mapper.Map<MailServerHistory>(mailServer);
                    return addAuditComplement(mailServerAudit, action, responsable);
                case MailCredential mailCredential:
                    var mailCredentialAudit = _mapper.Map<MailCredentialHistory>(mailCredential);
                    return addAuditComplement(mailCredentialAudit, action, responsable);
                default:
                    return null;
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
