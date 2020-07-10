using System;
using System.Collections.Generic;

namespace KivalitaAPI.DTOs
{
    public class LeadFilterDTO
    {
		public List<string> Sector { get; set; }
        public List<string> Position { get; set; }
        public List<string> Company { get; set; }
        public List<int?> UserId { get; set; }
        
    }
}