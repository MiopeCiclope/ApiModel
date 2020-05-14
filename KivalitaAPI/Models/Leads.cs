using KivalitaAPI.Interfaces;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
    public class Leads : IEntity
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string name { get; set; }
        public string position { get; set; }
        public string email { get; set; }
        public string company { get; set; }
        public string sector { get; set; }
        public string CompanyLinkedIn { get; set; }
        public string CompanySite { get; set; }
        public string phone { get; set; }
        public string linkedIn { get; set; }
    }
}
