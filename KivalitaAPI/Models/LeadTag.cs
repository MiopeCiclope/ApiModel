using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class LeadTag : IEntity
    {
        public int Id { get; set; }

        [ForeignKey("Leads")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int LeadId { get; set; }
        public Leads Lead { get; set; }

        [ForeignKey("Tag")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int TagId { get; set; }
        public Tag Tag { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public int UpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
