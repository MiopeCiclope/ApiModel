using KivalitaAPI.Interfaces;
using Sieve.Attributes;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace KivalitaAPI.Models
{
	public class LogTask : IEntity
    {
		public int Id { get; set; }
		public string Description { get; set; }
        public string Type {get;set;}
		[Sieve(CanFilter = true, CanSort = true)]
		[ForeignKey("Leads")]
		public int LeadId { get; set; }
		public Leads Leads { get; set; }

		[Sieve(CanFilter = true, CanSort = true)]
		public int TaskId { get; set; }

		[JsonIgnore]
		public int CreatedBy { get; set; }
		[JsonIgnore]
		public int UpdatedBy { get; set; }
		[JsonIgnore]
		public DateTime CreatedAt { get; set; }
		[JsonIgnore]
		public DateTime UpdatedAt { get; set; }

		public string[] GetDataBaseColumn()
		{
			return new string[9] {"Id"
								,"Description"
								,"Type"
								,"LeadId"
								,"TaskId"
								,"CreatedBy"
								,"UpdatedBy"
								,"CreatedAt"
								,"UpdatedAt"
							};
		}

	}
}
