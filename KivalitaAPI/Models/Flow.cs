using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
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
		public bool TagAsLost { get; set; }
		[Sieve(CanFilter = true, CanSort = true)]
		public bool EndLead { get; set; }
		[Sieve(CanFilter = true, CanSort = true)]
		public bool SendMoskit { get; set; }
		[Sieve(CanFilter = true, CanSort = true)]
		public bool SendRdStation { get; set; }
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
