using System.Collections.Generic;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class CompanyService : Service<Company, KivalitaApiContext, CompanyRepository>
    {
        public CompanyService(KivalitaApiContext context, CompanyRepository baseRepository) : base(context, baseRepository) {}

        public List<Company> WithOutOwner()
        {
            return baseRepository.WithOutOwner();
        }
    }
}