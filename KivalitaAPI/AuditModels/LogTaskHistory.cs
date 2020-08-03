using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.Models
{
	public class LogTaskHistory : IAuditTable
	{
		public int Id { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public int TaskId { get; set; }

		public int TableId { get; set; }
		public ActionEnum Action { get; set; }
		public int Responsable { get; set; }
		public DateTime Date { get; set; }
	}
}