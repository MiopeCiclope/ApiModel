using System;
using System.ComponentModel.DataAnnotations.Schema;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class Filter : IEntity {

        [Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Company { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Sector { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Position { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Email { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string StartDate { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string EndDate { get; set; }

		[Sieve(CanFilter = true, CanSort = true)] 
		public FilterTypeEnum type { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public LeadStatusEnum? Status { get; set; }

		public string SieveFilter { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public int Owner { get; set; }

		//[ForeignKey("Tag")]
		[Sieve(CanFilter = true, CanSort = true)]
		public string TagId { get; set; }

		[ForeignKey ("User")]
        [Sieve(CanFilter = true, CanSort = true)]
		public int UserId { get; set; }

		[ForeignKey("Flow")]
		[Sieve(CanFilter = true, CanSort = true)]
		public int? FlowId { get; set; }
		public Flow Flow { get; set; }

		[JsonIgnore]
		public virtual User User { get; set; }

		[JsonIgnore]
		public int CreatedBy { get; set; }

		[JsonIgnore]
		public int UpdatedBy { get; set; }

		[JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
		public DateTime CreatedAt { get; set; }

		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }

		public string GetSieveFilter()
		{
			string filter = string.Empty;

			if (this.Company != null) filter += $",CompanyName == {this.Company}";
			if (this.Sector != null) filter += $",CompanySector == {this.Sector}";
			if (this.Position != null) filter += $",Position == {this.Position}";
			if (this.UserId != null) filter += $",Owner == {this.UserId}";
			if (!String.IsNullOrEmpty(this.TagId)) filter += $",Tags == {this.TagId}";
			if (this.Email != null && this.Email != "bothEmail") filter += $",WithEmail == {(this.Email == "withEmail")}";
			if (this.StartDate != null) filter += $",createdAt>={ this.StartDate }";
			if (this.EndDate != null) filter += $",createdAt<={ this.EndDate }";
			return filter;
		}
	}
}
