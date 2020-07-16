using System.Collections.Generic;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class CompanyService : Service<Company, KivalitaApiContext, CompanyRepository>
    {
        public CompanyService(KivalitaApiContext context, CompanyRepository baseRepository) : base(context, baseRepository) {}

        public override Company Update(Company entity)
        {
            if(entity.shouldUpdateAllSectors)
            {
                var oldCompany = this.baseRepository.Get(entity.Id);
                var companiesWithSameSector = this.baseRepository
                                                    .GetBy(company => company.Sector == oldCompany.Sector);
                companiesWithSameSector.ForEach(comany => comany.Sector = entity.Sector);
                this.baseRepository.UpdateRange(companiesWithSameSector);
            }
            return base.Update(entity);
        }
        public List<Company> WithOutOwner()
        {
            return baseRepository.WithOutOwner();
        }
    }
}