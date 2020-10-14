using KivalitaAPI.Interfaces;
using Microsoft.Linq.Translations;
using Sieve.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class CompanyDatabaseDTO : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string Site { get; set; }
        public string LinkedIn { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string GetTableName()
        {
            return "Company";
        }
    }
}
