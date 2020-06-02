using System;
using System.Collections.Generic;
using KivalitaAPI.Models;

namespace KivalitaAPI.DTOs
{
    public class UserDTO
    {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedAt { get; set; }

		public List<Company> Company { get; set; }
	}
}