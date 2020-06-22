using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class Leads : IEntity
	{
		[JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
		public string Name { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
		public string Position { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
		public string Email { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
		public string PersonalEmail { get; set; }
		[Sieve(CanFilter = true, CanSort = true)]
		public string Phone { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
		public string LinkedIn { get; set; }
		[ForeignKey("Company")]
        [Sieve(CanFilter = true, CanSort = true)]
		public int? CompanyId { get; set; }
		public Company Company { get; set; }
		[JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }
		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public string CaptureDate {
			get { return dateExpression.Evaluate(this); }
		}
		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public int? Owner
		{
			get { return ownerExpression.Evaluate(this); }
		}
		private static readonly CompiledExpression<Leads, int?> ownerExpression 
						= DefaultTranslationOf<Leads>.Property(lead => lead.Owner).Is(lead => lead.Company != null ? lead.Company.UserId : 0);

		private static readonly CompiledExpression<Leads, string> dateExpression
						= DefaultTranslationOf<Leads>.Property(lead => lead.CaptureDate).Is(lead => lead.CreatedAt.Date.ToString());
	}
}
