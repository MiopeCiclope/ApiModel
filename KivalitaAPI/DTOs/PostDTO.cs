using KivalitaAPI.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace KivalitaAPI.DTOs
{
    public class PostDTO : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Article { get; set; }
        [Required(ErrorMessage = "A imagem é obrigatória")]
        public byte[] PostImage { get; set; }
    }
}
