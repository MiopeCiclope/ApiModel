using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class Post : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("Image")]
        [JsonIgnore]
        public int ImageId { get; set; }
        [JsonIgnore]
        public Image PostImage { get; set; }
        [ForeignKey("User")]
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
