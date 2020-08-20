using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Type { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string FileName { get; set; }

        public byte[] ImageData { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string ImageUrl { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Url { get; set; }

        [NotMapped]
        public string ImageString { get; set; }

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
