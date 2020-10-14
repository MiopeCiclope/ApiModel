using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class LeadDatabaseDTO : IEntity
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Position { get; set; }
		public string Email { get; set; }
		public string PersonalEmail { get; set; }
		public string Phone { get; set; }
		public string LinkedIn { get; set; }
		public string LinkedInPublic { get; set; }
		public LeadStatusEnum Status { get; set; }
		public int? CompanyId { get; set; }
		public int? FlowId { get; set; }
		public bool Deleted {get;set;}
		public bool DidGuessEmail { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }		
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }

		public string GetTableName()
		{
			return "Leads";
		}
	}
}
