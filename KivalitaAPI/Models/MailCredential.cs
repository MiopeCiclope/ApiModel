using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KivalitaAPI.Models
{
    public class MailCredential : IEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string email { get; set; }
        public string password { get; set; }
        [ForeignKey("MailServer")]
        [Sieve(CanFilter = true, CanSort = true)]
        public int MailServerId { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
