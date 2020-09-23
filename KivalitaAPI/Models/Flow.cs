using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class Flow : IEntity
    {
        [JsonIgnore]
		[Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string Name { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public bool actionForAllLeads { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public int leadGroupSize { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public bool isAutomatic { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public string DaysOfTheWeek { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public bool IsActive { get; set; }

		[ForeignKey("User")]
		[Sieve(CanFilter = true, CanSort = true)]
		public int? Owner { get; set; }
		public User User { get; set; }

		public List<Filter> Filter { get; set; }

		public List<FlowAction> FlowAction { get; set; }

		[JsonIgnore]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }
	}
}
