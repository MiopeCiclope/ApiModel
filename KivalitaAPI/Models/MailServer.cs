using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.Models
{
    public class MailServer : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SMTP { get; set; }
        public string POP { get; set; }
        public string IMAP { get; set; }
        public bool useSSL { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
