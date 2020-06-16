using KivalitaAPI.Enum;
using System;

namespace KivalitaAPI.Interfaces
{
    public interface IAuditTable
    {
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
