using KivalitaAPI.Interfaces;

namespace KivalitaAPI.Models
{
    public class Image : IEntity
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public string Type { get; set; }
    }
}
