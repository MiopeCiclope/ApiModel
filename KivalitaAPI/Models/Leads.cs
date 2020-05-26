using System;
using System.Text.Json.Serialization;
using KivalitaAPI.Interfaces;

namespace KivalitaAPI.Models {
	public class Leads : IEntity
	{
		[JsonIgnore]
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
		[JsonIgnore]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }
	}
}
