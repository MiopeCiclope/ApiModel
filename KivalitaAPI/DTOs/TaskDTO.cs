using System.ComponentModel.DataAnnotations;

namespace KivalitaAPI.DTOs
{
    public class TaskDTO
    {
        [Key]
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public int LeadId { get; set; }
        public string Email { get; set; }
    }
}
