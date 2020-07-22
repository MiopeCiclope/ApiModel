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
		public FilterTypeEnum type { get; set; }

		[ForeignKey ("User")]
        [Sieve(CanFilter = true, CanSort = true)]
		public int UserId { get; set; }

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

	}
}
