using System;
using System.Collections.Generic;
using KivalitaAPI.Interfaces;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using KivalitaAPI.Enum;

namespace KivalitaAPI.AuditModels
{
    public class UserHistory : IAuditTable
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
        public int TableId { get; set; }
    }
}
