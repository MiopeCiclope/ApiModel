using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class FlowHistory : IAuditTable
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public bool actionForAllLeads { get; set; }
		public int leadGroupSize { get; set; }
		public bool isAutomatic { get; set; }
		public string DaysOfTheWeek { get; set; }
		public bool IsActive { get; set; }
		public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
		public bool Deleted { get; set; }		
		public DateTime Date { get; set; }
    }
}
