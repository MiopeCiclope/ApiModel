using System;
using System.Collections.Generic;
using System.Linq;
using KivalitaAPI.Interfaces;
using KivalitaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KivalitaAPI.Repositories
{
    public class CompanyRepository: Repository<Company, DbContext>
    {
        public CompanyRepository(DbContext context) : base(context) {}

        public override Company Add(Company company)
        {
            var companyFound = this.GetBy(
                c => c.LinkedIn == company.LinkedIn ||
                c.Name == company.Name).FirstOrDefault();

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
            return base.GetAll().FindAll(c => c.UserId == null);
        }
    }
}
