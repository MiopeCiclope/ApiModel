using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using KivalitaAPI.Interfaces;

namespace KivalitaAPI.Models {
	public class Filter : IEntity {

		[JsonIgnore]
		public int Id { get; set; }
		public string Name { get; set; }
		public string Company { get; set; }
		public string Sector { get; set; }
		public string Position { get; set; }
		public string Email { get; set; }

		[ForeignKey ("User")]
		public int UserId { get; set; }

		[JsonIgnore]
		public virtual User User { get; set; }

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
