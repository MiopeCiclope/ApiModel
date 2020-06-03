using System;

namespace KivalitaAPI.DTOs
{
    public class LeadDTO
    {
		public int Id { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public string Email { get; set; }
		public string Company { get; set; }
		public string Sector { get; set; }
		public string CompanySite { get; set; }
		public string CompanyLinkedIn { get; set; }
		public string Phone { get; set; }
		public string LinkedIn { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}