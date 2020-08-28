using System;
using System.ComponentModel.DataAnnotations.Schema;
using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;

namespace KivalitaAPI.Models {
	public class MailSignature : IEntity {

        [Sieve(CanFilter = true, CanSort = true)]
		public int Id { get; set; }

		[ForeignKey ("User")]
        [Sieve(CanFilter = true, CanSort = true)]
		public int UserId { get; set; }

		[JsonIgnore]
		public virtual User User { get; set; }

		public string? Signature { get; set; }

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
