using KivalitaAPI.Interfaces;
using System.Text.Json.Serialization;
namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
    }
}
