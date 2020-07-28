using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.Models
{
	public class TaskNoteHistory : IAuditTable
	{
		public int Id { get; set; }
		public string description { get; set; }
		public int FlowTaskId { get; set; }

		public int TableId { get; set; }
		public ActionEnum Action { get; set; }
		public int Responsable { get; set; }
		public DateTime Date { get; set; }
	}
}