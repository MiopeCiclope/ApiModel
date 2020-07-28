using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class TaskNote : IEntity
    {
		public int Id { get; set; }

		public string description { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		[ForeignKey("FlowTask")]
		public int FlowTaskId { get; set; }
		public FlowTask FlowTask { get; set; }

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
