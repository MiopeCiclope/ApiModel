using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class Leads : IEntity
	{
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

		[Sieve(CanFilter = true, CanSort = true)]
		public string LinkedInPublic { get; set; }

		[ForeignKey("Company")]
        [Sieve(CanFilter = true, CanSort = true)]
		public int? CompanyId { get; set; }
		public Company Company { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public bool Deleted {get;set;}

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
		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public string? CompanyName
		{
			get { return companyNameExpression.Evaluate(this); }
		}
		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public string? CompanySector
		{
			get { return CompanySectorExpression.Evaluate(this); }
		}
		[JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public bool WithEmail
		{
			get { return WithEmailExpression.Evaluate(this); }
		}

		private static readonly CompiledExpression<Leads, int?> ownerExpression 
						= DefaultTranslationOf<Leads>.Property(lead => lead.Owner).Is(lead => lead.Company != null ? lead.Company.UserId : 0);

		private static readonly CompiledExpression<Leads, string> dateExpression
						= DefaultTranslationOf<Leads>.Property(lead => lead.CaptureDate).Is(lead => lead.CreatedAt.Date.ToString());

		private static readonly CompiledExpression<Leads, string> companyNameExpression
						= DefaultTranslationOf<Leads>.Property(lead => lead.CompanyName).Is(
							lead => lead.Company != null ? lead.Company.Name : "");

		private static readonly CompiledExpression<Leads, string> CompanySectorExpression
						= DefaultTranslationOf<Leads>.Property(lead => lead.CompanySector).Is(
							lead => lead.Company != null ? lead.Company.Sector : "");

		private static readonly CompiledExpression<Leads, bool> WithEmailExpression
						= DefaultTranslationOf<Leads>.Property(lead => lead.WithEmail).Is(lead => lead.Email != null);
	}
}
