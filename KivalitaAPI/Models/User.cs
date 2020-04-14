using KivalitaAPI.Interfaces;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class User : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
