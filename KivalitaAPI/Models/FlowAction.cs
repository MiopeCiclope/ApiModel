using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class FlowAction : IEntity
    {
        [JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string Type { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public int TemplateId { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public int afterDays { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public bool Done { get; set; }

		public int FlowId { get; set; }
		public Flow Flow { get; set; }

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
