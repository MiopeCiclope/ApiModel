using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class PostHistory : IAuditTable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        public bool Published { get; set; }
        public int ImageId { get; set; }
        public int AuthorId { get; set; }
        public LanguageEnum Language { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
