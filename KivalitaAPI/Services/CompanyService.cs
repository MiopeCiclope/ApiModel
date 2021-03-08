using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class CompanyService : Service<Company, KivalitaApiContext, CompanyRepository>
    {
        LeadsService leadsService;
        IMapper _mapper;
        CompanyDTORepository companyDtoRepository;

        public CompanyService(
            KivalitaApiContext context,
            CompanyRepository baseRepository,
            LeadsService leadsService,
            IMapper mapper,
            CompanyDTORepository _companyDTORepository
        ) : base(context, baseRepository)
        {
            this.leadsService = leadsService;
            this._mapper = mapper;
            this.companyDtoRepository = _companyDTORepository;
        }

        public override Company Update(Company entity)
        {
            var oldCompany = this.baseRepository.Get(entity.Id);
            if (oldCompany.LinkedIn != entity.LinkedIn || oldCompany.Name != entity.Name)
            {
                var companyExists = this.baseRepository
                                            .GetBy(company =>
                                                company.Id != entity.Id
                                                && (
                                                    company.LinkedIn?.ToLower() == entity.LinkedIn?.ToLower() 
                                                    || company.Name?.ToLower() == entity.Name?.ToLower())
                                                    )
                                            .Any();

                if (companyExists) throw new Exception("Já existe empresa com esse nome ou Linked In");
            }

            if(entity.shouldUpdateAllSectors)
            {
                var companiesWithSameSector = this.baseRepository
                                                    .GetBy(company => company.Sector == oldCompany.Sector);
                companiesWithSameSector.ForEach(comany => comany.Sector = entity.Sector);

                var bulkListCompany = _mapper.Map<List<CompanyDatabaseDTO>>(companiesWithSameSector);
                companyDtoRepository.UpdateRange(bulkListCompany);
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