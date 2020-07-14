using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class FlowLeads
	{
		public int LeadsId { get; set; }
		public int FlowId { get; set; }

		public virtual Flow Flow { get; set; }
		public virtual Leads Leads { get; set; }
	}
}
