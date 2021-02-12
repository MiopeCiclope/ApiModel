using KivalitaAPI.Models;
using System.Collections.Generic;

namespace KivalitaAPI.Equitables
{
    public class CompanyEquitable : IEqualityComparer<Company>
    {
        public bool Equals(Company x, Company y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Company obj)
        {
            return obj.Id;
        }
    }
}
