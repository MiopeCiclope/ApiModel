﻿using System;
using System.Collections.Generic;
using KivalitaAPI.Interfaces;
using System.Text.Json.Serialization;
using Sieve.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace KivalitaAPI.Models
{
    public class User : IEntity
    {
        [JsonIgnore]
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

        public string Timezone { get; set; }

        [NotMapped]
        public bool? LinkedMicrosoftGraph { get; set; }

        public virtual ICollection<Filter> Filters { get; set; }

        [JsonIgnore]
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
    }
}
