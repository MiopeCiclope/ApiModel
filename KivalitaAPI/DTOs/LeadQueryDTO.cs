using System;

namespace KivalitaAPI.DTOs
{
    public class LeadQueryDTO
    {
		public int Page { get; set; }
        public string Search { get; set; }
        public string Sector { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public bool WithEmail { get; set; }
        public bool WithoutEmail { get; set; }
        public DateTime? Date { get; set; }
        public int? UserId { get; set; }
    }
}