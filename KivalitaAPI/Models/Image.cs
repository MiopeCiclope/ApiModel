using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [JsonIgnore]
        public byte[] ImageData { get; set; }

        [JsonIgnore]
        public byte[] ThumbnailData { get; set; }

        [NotMapped]
        public string ImageString { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Type { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Url { get; set; }
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
