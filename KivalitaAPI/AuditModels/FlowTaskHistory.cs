using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class FlowTaskHistory : IAuditTable
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public DateTime ScheduledTo { get; set; }
        public int LeadId { get; set; }
        public int FlowActionId { get; set; }

		public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
