using KivalitaAPI.Enum;
using KivalitaAPI.Interfaces;
using System;

namespace KivalitaAPI.AuditModels
{
    public class ImageHistory : IAuditTable
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public byte[] ThumbnailData { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public int TableId { get; set; }
        public ActionEnum Action { get; set; }
        public int Responsable { get; set; }
        public DateTime Date { get; set; }
    }
}
