using KivalitaAPI.Interfaces;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class WpRdStation : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string FormData { get; set; }
    }
}
