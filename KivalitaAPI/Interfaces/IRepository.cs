using Sieve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KivalitaAPI.Interfaces
{
    public interface IRepository<T> where T : class, IEntity
    {
        List<T> GetAll();
        List<T> GetAll_v2(SieveModel filterQuery);
        T Get(int id);
        T Add(T entity);
        List<T> AddRange(List<T> entities);
        T Update(T entity);
        T Delete(int id);
        List<T> GetBy(Func<T, bool> condition);
    }
}
