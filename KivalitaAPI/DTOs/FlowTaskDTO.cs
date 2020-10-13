using KivalitaAPI.Interfaces;
using System;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class FlowTaskDTO : IEntity
	{
		public int Id { get; set; }
		public string Status { get; set; } 
		public DateTime? ScheduledTo { get; set; }
		public int LeadId { get; set; }
		[JsonIgnore]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }

		public string GetTableName()
		{
			return "FlowTask";
		}
	}
}
