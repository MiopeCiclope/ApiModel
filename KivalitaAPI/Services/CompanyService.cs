using System;
using System.Collections.Generic;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class CompanyService : Service<Company, KivalitaApiContext, CompanyRepository>
    {
        LeadsService leadsService;

        public CompanyService(
            KivalitaApiContext context,
            CompanyRepository baseRepository,
            LeadsService leadsService
        ) : base(context, baseRepository)
        {
            this.leadsService = leadsService;
        }

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

        public override Company Delete(int id, int userId)
        {
            if (leadsService.HasCompany(id))
            {
                throw new Exception("Não é possível excluir a empresa pois existe leads relacionadas!");
            }

            return baseRepository.Delete(id, userId);
        }
    }
}