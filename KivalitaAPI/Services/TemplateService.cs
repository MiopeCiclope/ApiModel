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
        TemplateTransformService templateTransformService;

        public TemplateService(
            KivalitaApiContext context,
            TemplateRepository baseRepository,
            CategoryRepository categoryRepository,
            FlowService flowService,
            TemplateTransformService templateTransformService
        ) : base(context, baseRepository)
        {
            _categoryRepository = categoryRepository;
            this.flowService = flowService;
            this.templateTransformService = templateTransformService;
        }

        public override Template Add(Template entity)
        {
            if (!templateTransformService.IsValid(entity.Content))
            {
                throw new Exception("Erro no formato do template: pelo menos uma ou mais váriaveis estão definidas incorretamente.");
            }

            var existingCategory = _categoryRepository.Get(entity.CategoryId);
            if (existingCategory == null)
                throw new Exception("Categoria não encontrada");

            entity.Category = existingCategory;

            return base.Add(entity);
        }

        public override Template Update(Template entity)
        {
            if (!templateTransformService.IsValid(entity.Content))
            {
                throw new Exception("Erro no formato do template: pelo menos uma ou mais váriaveis estão definidas incorretamente.");
            }

            return base.Update(entity);
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
