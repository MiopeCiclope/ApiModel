using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class MailServerHistory : IAuditTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SMTP { get; set; }
        public string POP { get; set; }
        public string IMAP { get; set; }
        public bool useSSL { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
