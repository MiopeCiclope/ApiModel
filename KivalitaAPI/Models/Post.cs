using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using Newtonsoft.Json;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class Post : IEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Title { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Slug { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public string Article { get; set; }

        [Sieve(CanFilter = true, CanSort = false)]
        public bool Published { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("Image")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int ImageId { get; set; }

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

        [Sieve(CanFilter = true, CanSort = true)]
        public int? LinkId { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public int UpdatedBy { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }
    }
}
