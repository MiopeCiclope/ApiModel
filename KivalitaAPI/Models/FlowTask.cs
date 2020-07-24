using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class FlowTask : IEntity
    {
		public int Id { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string Status { get; set; } // pending, finished, error

		[Sieve(CanFilter = true, CanSort = true)]
		public DateTime? ScheduledTo { get; set; }

		[ForeignKey("Leads")]
		public int LeadId { get; set; }
		public Leads Leads { get; set; }

		[ForeignKey("FlowAction")]
		public int FlowActionId { get; set; }
		public FlowAction FlowAction { get; set; }

		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public int? Owner
		{
			get { return ownerExpression.Evaluate(this); }
		}

		[JsonIgnore]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }

		private static readonly CompiledExpression<FlowTask, int?> ownerExpression
				= DefaultTranslationOf<FlowTask>.Property(task => task.Owner).Is(task => task.Leads.Company.UserId != null ? task.Leads.Company.UserId : 0);
	}
}
