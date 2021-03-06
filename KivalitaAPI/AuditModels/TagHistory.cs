using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class TagHistory : IAuditTable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
