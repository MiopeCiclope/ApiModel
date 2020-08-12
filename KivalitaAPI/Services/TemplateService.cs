using System;
using KivalitaAPI.Data;
using KivalitaAPI.Models;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{

    public class TemplateService : Service<Template, KivalitaApiContext, TemplateRepository>
    {
        CategoryRepository _categoryRepository;
        FlowService flowService;

        public TemplateService(
            KivalitaApiContext context,
            TemplateRepository baseRepository,
            CategoryRepository categoryRepository,
            FlowService flowService
        ) : base(context, baseRepository)
        {
            _categoryRepository = categoryRepository;
            this.flowService = flowService;
        }

        public override Template Add(Template entity)
        {
            var existingCategory = _categoryRepository.Get(entity.CategoryId);
            if (existingCategory == null)
                throw new Exception("Categoria não encontrada");

            entity.Category = existingCategory;

            return base.Add(entity);
        }

        public override Template Delete(int id, int userId)
        {
            if (flowService.HasTemplate(id))
            {
                throw new Exception("Não é possível excluir o Template pois ele está sendo usado!");
            }

            return baseRepository.Delete(id, userId);
        }
    }
}
