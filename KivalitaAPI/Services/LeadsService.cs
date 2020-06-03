using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services {

	public class LeadsService : Service<Leads, KivalitaApiContext, LeadsRepository> {

		CompanyRepository companyRepository;

		public LeadsService (KivalitaApiContext context, LeadsRepository baseRepository, CompanyRepository companyRepository) : base (context, baseRepository) {
			this.companyRepository = companyRepository;
		}

		public override List<Leads> GetAll()
		{
			// TODO - Get only 200 leads
			return base.GetAll()
				.Take(200)
				.Select(lead => { 
					lead.CreatedAt = lead.CreatedAt.Date;
					return lead; 
				})
				.ToList();
		}
		public List<GroupOwnerLeadDTO> GetDates()
		{
			// TODO - Get only 200 leads
			return this.baseRepository.GetAll().Take(200).GroupBy(lead => lead.Company?.UserId, (key, func) => new GroupOwnerLeadDTO
			{
				Dates = func.Select(lead => lead.CreatedAt.Date).Distinct().OrderByDescending(value => value).ToList(),
				UserId = key
			}).ToList();
		}

        public Boolean LeadExists(string linkedInID)
        {
			var leadSearch = this.baseRepository.GetBy(storedLead => storedLead.LinkedIn == linkedInID);

			return leadSearch.FirstOrDefault() != null ? true : false;
		}


		public virtual List<Leads> SaveRange(List<LeadDTO> leadDTOs)
        {
			List<Leads> leads = new List<Leads> { };

			foreach(LeadDTO lead in leadDTOs)
            {
				Company company = new Company
				{
					Name = lead.Company,
					Sector = lead.Sector,
					Site = lead.CompanySite,
					LinkedIn = lead.CompanyLinkedIn,
					CreatedBy = lead.CreatedBy,
					CreatedAt = lead.CreatedAt
				};

				company = this.companyRepository.Add(company);

				Leads newLead = new Leads
				{
					Name = lead.Name,
					Email = lead.Email,
					Phone = lead.Phone,
					Position = lead.Position,
					LinkedIn = lead.LinkedIn,
					CreatedAt = lead.CreatedAt,
					CreatedBy = lead.CreatedBy,
					Company = company
				};

				leads.Add(this.baseRepository.Add(newLead));

            }

			return leads;
		}
	}
}