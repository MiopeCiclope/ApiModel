using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class Token : IEntity
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string RefreshToken { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
