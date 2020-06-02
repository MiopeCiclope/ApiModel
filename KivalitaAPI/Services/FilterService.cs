using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class FilterService : Service<Filter, KivalitaApiContext, FilterRepository>
    {
        public FilterService(KivalitaApiContext context, FilterRepository baseRepository) : base(context, baseRepository) { }

        public override List<Filter> GetAll()
        {
            return base.GetAll()
                .Select(filter =>
                {
                    filter.CreatedAt = filter.CreatedAt.Date;
                    return filter;
                })
                .ToList();
        }
        public Boolean FilterExists(string nameOfFilter)
        {
            var filterSearch = this.baseRepository.GetBy(storedFilter => storedFilter.Name == nameOfFilter);

            return filterSearch.FirstOrDefault() != null ? true : false;
        }

    }
}
