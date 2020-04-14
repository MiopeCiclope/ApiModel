using KivalitaAPI.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
    }
}
