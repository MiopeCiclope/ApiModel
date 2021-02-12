using KivalitaAPI.Models;
using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KivalitaAPI.DTOs
{
    public class LeadBulkDTO
    {
        public string Sector { get; set; }
        public List<int> Tags { get; set; }
        public List<Leads> LeadList { get; set; }
        public SieveModel filter { get; set; }
    }
}
