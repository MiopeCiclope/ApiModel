using KivalitaAPI.Interfaces;
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
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string Site { get; set; }
        public string LinkedIn { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public int UpdatedBy { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }

        [IgnoreDataMember]
        public ICollection<Leads> Leads { get; set; }
    }
}
