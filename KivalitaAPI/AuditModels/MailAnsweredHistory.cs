using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class MailAnsweredHistory : IAuditTable
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int TaskId { get; set; }
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public string BodyPreview { get; set; }
        public string BodyContent { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }

        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
