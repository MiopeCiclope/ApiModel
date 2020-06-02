using System;
using System.ComponentModel.DataAnnotations.Schema;
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

		[ForeignKey("Company")]
		public int CompanyId { get; set; }

		public Company Company { get; set; }
	}
}
