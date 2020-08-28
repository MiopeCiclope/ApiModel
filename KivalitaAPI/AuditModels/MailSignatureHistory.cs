using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class MailSignatureHistory : IAuditTable
    {
		public int Id { get; set; }
        public int UserId { get; set; }
        public string? Signature { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
