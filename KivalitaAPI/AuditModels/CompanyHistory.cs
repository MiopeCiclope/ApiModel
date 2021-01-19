using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class CompanyHistory : IAuditTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sector { get; set; }
        public string Site { get; set; }
        public string LinkedIn { get; set; }
        public int? UserId { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
