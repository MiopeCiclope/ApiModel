using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class Company : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Sector { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Site { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string LinkedIn { get; set; }
        [ForeignKey("User")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int? UserId { get; set; }
        public User User { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public int UpdatedBy { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }

        [IgnoreDataMember]
        public ICollection<Leads> Leads { get; set; }

        [JsonIgnore]
        [NotMapped]
        public bool shouldUpdateAllSectors { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int? Owner
        {
            get { return ownerExpression.Evaluate(this); }
        }
        private static readonly CompiledExpression<Company, int?> ownerExpression
                = DefaultTranslationOf<Company>.Property(company => company.Owner).Is(company => company.UserId != null ? company.UserId : 0);
    }
}
