using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Common;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Linq.Translations;
using Sieve.Models;
using Sieve.Services;

namespace KivalitaAPI.Repositories
{
    public class CompanyRepository: Repository<Company, DbContext, SieveProcessor>
    {
        public CompanyRepository(DbContext context, SieveProcessor filterProcessor) : base(context, filterProcessor) {}

        public override Company Add(Company company)
        {
            Company companyFound = null;

            if (!String.IsNullOrEmpty(company.LinkedIn)) {
                companyFound = this.GetBy(
                    c => c.LinkedIn == company.LinkedIn).FirstOrDefault();
            }

            if (companyFound == null)
            {
                companyFound = this.GetBy(
                    c => c.Name == company.Name).FirstOrDefault();
            }

            if (companyFound != null)
            {
                return companyFound;
            }

            return base.Add(company);

        }

        public List<Company> getByUserId(int userId)
        {
            return this.GetBy(c => c.UserId == userId).ToList();
        }

        public List<Company> WithOutOwner()
        {
            return QueryWithOutOwner().ToList();
        }

        public IQueryable<Company> QueryWithOutOwner()
        {
            return context.Set<Company>().Where(c => c.UserId == null);
        }

        public IQueryable<Company> QueryByUserId(int userId)
        {
            return context.Set<Company>().Where(c => c.UserId == userId);
        }

        public override QueryResult<Company> GetAll_v2(SieveModel filterQuery)
        {
            var result = context.Set<Company>()
                                .Include(c => c.User)
                                .AsNoTracking();

            result = this.filterProcessor.Apply(filterQuery, result, applyPagination: false).WithTranslations();
            var total = result.Count();

            result = this.filterProcessor.Apply(filterQuery, result, applyFiltering: false, applySorting: false);

            return new QueryResult<Company>
            {
                Items = result.ToList(),
                TotalItems = total,
            };
        }
    }
}
