using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services {

	public class LeadsService : Service<Leads, KivalitaApiContext, LeadsRepository> {
		public LeadsService (KivalitaApiContext context, LeadsRepository baseRepository) : base (context, baseRepository) { }

		public override List<Leads> GetAll()
		{
			return base.GetAll()
				.Select(lead => { 
					lead.CreatedAt = lead.CreatedAt.Date; 
					return lead; 
				})
				.ToList();
		}
		public List<DateTime> GetDates()
		{
			return this.baseRepository.GetAll()
						.Select(lead => lead.CreatedAt.Date)
						.Distinct()
						.OrderBy(x => x)
						.ToList();
		}

	}
}
