using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class Post : IEntity
    {
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Title { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Article { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public bool Published { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Image")]
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public int ImageId { get; set; }
        [JsonIgnore]
        public Image PostImage { get; set; }
        [ForeignKey("User")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int AuthorId { get; set; }
        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public User Author { get; set; }
        [NotMapped]
        public string ImageData { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public LanguageEnum Language { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public bool isNews { get; set; }
        [JsonIgnore]
        public int CreatedBy { get; set; }
        [JsonIgnore]
        public int UpdatedBy { get; set; }
        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
