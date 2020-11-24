using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class MailTrack : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [ForeignKey("Leads")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int LeadId { get; set; }

        [ForeignKey("FlowTask")]
        public int TaskId { get; set; }
        public FlowTask FlowTask { get; set; }

        [JsonIgnore]
        public Leads Lead { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public int UpdatedBy { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
