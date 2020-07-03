using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class CategoryService : Service<Category, KivalitaApiContext, CategoryRepository>
    {
        public CategoryService(KivalitaApiContext context, CategoryRepository baseRepository) : base(context, baseRepository) { }
    }
}
