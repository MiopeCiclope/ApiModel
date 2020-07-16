using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class MailTrackHistory : IAuditTable
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int TaskId { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
