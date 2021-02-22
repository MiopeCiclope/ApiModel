using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class MailCredentialHistory : IAuditTable
    {
        public int Id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int MailServerId { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
