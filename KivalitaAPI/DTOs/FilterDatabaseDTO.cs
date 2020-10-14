using System;
using System.ComponentModel.DataAnnotations.Schema;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models
{
	public class FilterDatabaseDTO : IEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Company { get; set; }
		public string Sector { get; set; }
		public string Position { get; set; }
		public string Email { get; set; }
		public string Date { get; set; }
		public FilterTypeEnum type { get; set; }
		public LeadStatusEnum? Status { get; set; }
		public int Owner { get; set; }
		public string TagId { get; set; }
		public int UserId { get; set; }
		public int? FlowId { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public string GetTableName()
		{
			return "Filter";
		}
	}
}
