using System;
using System.Collections.Generic;
using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace KivalitaAPI.Models
{
    public class User : IEntity
    {
        [Sieve(CanFilter = true, CanSort = true)]
        public int Id { get; set; }
        [Sieve(CanFilter = true, CanSort=true)]
        public string FirstName { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string LastName { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Email { get; set; }
        [Sieve(CanFilter = true, CanSort = true)]
        public string Role { get; set; }

        [Sieve(CanFilter = true, CanSort = true)]
        public bool Active { get; set; }

        public string Timezone { get; set; }

        [NotMapped]
        public bool? LinkedMicrosoftGraph { get; set; }

        public virtual ICollection<Filter> Filters { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public int CreatedBy { get; set; }

        [JsonIgnore]
        public int UpdatedBy { get; set; }

        [JsonIgnore]
        [Sieve(CanFilter = true, CanSort = true)]
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public ICollection<Company> Company { get; set; }

        public int? MailSignatureId { get; set; }
        public MailSignature? MailSignature { get; set; }
    }
}
