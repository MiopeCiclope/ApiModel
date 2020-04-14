using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class Post : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("Image")]
        public int ImageId { get; set; }
        public Image PostImage { get; set; }
        [ForeignKey("User")]
        public int AuthorId { get; set; }
        public User Author { get; set; }
        [NotMapped]
        public string ImageData { get; set; }
    }
}
