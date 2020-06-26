using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using KivalitaAPI.Interfaces;
using Sieve.Attributes;

namespace KivalitaAPI.Models
{
    public class Job : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public bool Published { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Image")]
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int ImageId { get; set; }

        [JsonIgnore]
        public Image JobImage { get; set; }

        [ForeignKey("User")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int AuthorId { get; set; }

        [JsonIgnore]
        public User Author { get; set; }

        [NotMapped]
        public string ImageData { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public int UpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
