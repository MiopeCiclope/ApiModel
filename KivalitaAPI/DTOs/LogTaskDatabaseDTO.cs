using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.DTOs
{
    public class LogTaskDatabaseDTO : IEntity
    {
		public int Id { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public int LeadId { get; set; }
		public int TaskId { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public string GetTableName()
		{
			return "LogTask";
		}
	}
}
