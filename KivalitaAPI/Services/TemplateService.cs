using System;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class TemplateService : Service<Template, KivalitaApiContext, TemplateRepository>
    {
        CategoryRepository _categoryRepository;

        public TemplateService(
            KivalitaApiContext context,
            TemplateRepository baseRepository,
            CategoryRepository categoryRepository
        ) : base(context, baseRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override Template Add(Template entity)
        {
            var existingCategory = _categoryRepository.Get(entity.CategoryId);
            if (existingCategory == null)
                throw new Exception("Categoria não encontrada");

            entity.Category = existingCategory;

            return base.Add(entity);
        }
    }
}
