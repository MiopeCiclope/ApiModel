using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.DTOs
{
	public class FlowTaskQueryDTO
    {
		public int Id { get; set; }
		public string Status { get; set; }
		public DateTime? ScheduledTo { get; set; }
		public int LeadId { get; set; }
		public int LeadsId { get; set; }
		public string LeadName { get; set; }
		public int? FlowActionId { get; set; }
		public int? Owner { get; set; }
		public int CreatedBy { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public int CompanyId { get; set; }
		public string CompanyName { get; set; }
		public int FlowActionFlowId { get; set; }
		public string FlowActionType { get; set; }
		public int FlowId { get; set; } 
        public string FlowName { get; set; }
		public int UserId { get; set; } 
        public string UserFirstName { get; set; }
	}
}
