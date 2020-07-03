using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace KivalitaAPI.Models
{
    public class Template : IEntity
    {
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Name { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Type { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Subject { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Content { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

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
