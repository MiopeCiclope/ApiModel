﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using KivalitaAPI.Interfaces;

namespace KivalitaAPI.Models {
	public class User : IEntity {
		[JsonIgnore]
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }
		public virtual ICollection<Filter> Filters { get; set; }

		[JsonIgnore]
		public string Password { get; set; }

		[JsonIgnore]
		public int CreatedBy { get; set; }

		[JsonIgnore]
		public int UpdatedBy { get; set; }

		[JsonIgnore]
		public DateTime CreatedAt { get; set; }

		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }
	}
}
