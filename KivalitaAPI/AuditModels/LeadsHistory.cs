using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class LeadsHistory : IAuditTable
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public string Email { get; set; }
		public string PersonalEmail { get; set; }
		public string Phone { get; set; }
		public string LinkedIn { get; set; }
		public string LinkedInPublic { get; set; }
		public LeadStatusEnum Status { get; set; }
		public int? CompanyId { get; set; }
		public int? FlowId { get; set; }
		public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
		public bool Deleted { get; set; }
	}
}
