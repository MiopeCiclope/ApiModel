using System;
using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class PreLead : IEntity
	{
		[Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
		public string Email { get; set; }

		[JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
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
