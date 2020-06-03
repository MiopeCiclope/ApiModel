using System;
using System.Collections.Generic;

namespace KivalitaAPI.DTOs
{
    public class GroupOwnerLeadDTO
    {
		public int? UserId { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}