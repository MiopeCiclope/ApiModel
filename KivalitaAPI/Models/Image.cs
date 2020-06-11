using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public byte[] ImageData { get; set; }

        [NotMapped]
        public string ImageString { get; set; }

        public string Type { get; set; }
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
