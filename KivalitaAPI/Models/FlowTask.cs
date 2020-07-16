using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class FlowTask : IEntity
    {
		public int Id { get; set; }
		public string Status { get; set; } // pending, finished, error
		public DateTime? ScheduledTo { get; set; }

		[ForeignKey("Leads")]
		public int LeadId { get; set; }
		public Leads Leads { get; set; }

		[ForeignKey("FlowAction")]
		public int FlowActionId { get; set; }
		public FlowAction FlowAction { get; set; }

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
