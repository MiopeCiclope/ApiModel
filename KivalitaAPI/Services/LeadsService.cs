using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Common;
using KivalitaAPI.Data;
using KivalitaAPI.DTOs;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;
using Sieve.Models;

namespace KivalitaAPI.Services {

	public class LeadsService : Service<Leads, KivalitaApiContext, LeadsRepository> {

		CompanyRepository companyRepository;

		public LeadsService (KivalitaApiContext context, LeadsRepository baseRepository, CompanyRepository companyRepository) : base (context, baseRepository) {
			this.companyRepository = companyRepository;
		}

		public QueryResult<Leads> FetchAll(LeadQueryDTO leadQuery)
		{
			if (leadQuery.Page <= 0)
			{
				leadQuery.Page = 1;
			}

			return this.baseRepository.FetchAll(leadQuery);
		}

		public override List<Leads> GetAll()
		{
			return base.GetAll()
				.Select(lead => { 
					lead.CreatedAt = lead.CreatedAt.Date;
					return lead; 
				})
				.ToList();
		}
		public List<GroupOwnerLeadDTO> GetDates()
		{
			return this.baseRepository.GetAll().GroupBy(lead => lead.Company?.UserId, (key, func) => new GroupOwnerLeadDTO
			{
				Dates = func.Select(lead => lead.CreatedAt.Date).Distinct().OrderByDescending(value => value).ToList(),
				UserId = key
			}).ToList();
		}

		public LeadFilterDTO GetFilter(LeadQueryDTO leadQuery)
		{
			var leads = this.baseRepository.FetchFilterAll(leadQuery);
			var positions = leads.Select(lead => lead.Position).Distinct().OrderBy(value => value).ToList();
			var sector = leads.Select(lead => lead.Company?.Sector).Distinct().OrderBy(value => value).ToList();
			var companies = leads.Select(lead => lead.Company?.Name).Distinct().OrderBy(value => value).ToList();
			var userId = leads.Select(lead => lead.Company?.UserId).Distinct().OrderBy(value => value).ToList();
			return new LeadFilterDTO
			{
				Position = positions,
				Sector = sector,
				Company = companies,
				UserId = userId
			};
		}

        public List<Leads> GetMailFromFlow(int flowId)
        {
			return new List<Leads> { baseRepository.Get(19890) };
        }

        public bool LeadExists(string linkedInID)
        {
			var leadSearch = this.baseRepository.GetBy(
				storedLead => storedLead.LinkedIn == linkedInID || storedLead.LinkedInPublic == linkedInID);

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
					PersonalEmail = lead.PersonalEmail,
					Phone = lead.Phone,
					Position = lead.Position,
					LinkedIn = lead.LinkedIn,
					LinkedInPublic = lead.LinkedInPublic,
					CreatedAt = lead.CreatedAt,
					CreatedBy = lead.CreatedBy,
					Company = company
				};

				leads.Add(this.baseRepository.Add(newLead));

            }

			return leads;
		}

        public int GetDailyLeads()
        {
			return this.baseRepository
					.GetBy(lead => lead.CreatedAt.Date == DateTime.Now.Date)
					.Count();
        }

		public void LinkWithoutOwnerToUser(List<int> userIds)
		{
			var usersWithLeadCount = new List<UserWithLeadCountDTO> { };

			foreach (int userId in userIds)
			{
				var leadCount = companyRepository.QueryByUserId(userId)
					.SelectMany(c => c.Leads).Count();

				usersWithLeadCount.Add(new UserWithLeadCountDTO
				{
					UserId = userId,
					LeadCount = leadCount
				});
			}

			var companiesWithLeadCount = companyRepository.QueryWithOutOwner().Select(company => new
			{
				Company = company,
				LeadCount = company.Leads.Count()
			}).OrderBy(c => c.LeadCount).ToList();

			var companiesWithLeadCountSplit = SplitList(companiesWithLeadCount, usersWithLeadCount.Count());
			List<Company> companiesToUpdate = new List<Company> { };

			foreach (var groupList in companiesWithLeadCountSplit)
			{
				var index = 0;
				var usersWithLeadCountOrdered = usersWithLeadCount
					.OrderByDescending(u => u.LeadCount).ToList();

				foreach (var group in groupList)
				{
					var user = usersWithLeadCountOrdered[index];

					group.Company.UserId = user.UserId;
					companiesToUpdate.Add(group.Company);

					user.LeadCount += group.LeadCount;
					index += 1;
				}
			}

			companyRepository.UpdateRange(companiesToUpdate);
		}

		private List<List<T>> SplitList<T>(List<T> list, int number)
		{
			return list
				.Select((x, i) => new { Index = i, Value = x })
				.GroupBy(x => x.Index / number)
				.Select(x => x.Select(v => v.Value).ToList())
				.ToList();
		}

		public new QueryResult<Leads> GetAll_v2(SieveModel filterQuery)
		{
			return this.baseRepository.GetAll_v2(filterQuery);
		}
	}
}