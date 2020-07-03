using DotLiquid;
using KivalitaAPI.Repositories;

namespace KivalitaAPI.Services
{
    public class TemplateTransformService
    {
        private readonly TemplateRepository _baseRepository;

        public TemplateTransformService(TemplateRepository repository)
        {
            this._baseRepository = repository;
        }

        public string Transform(int templateId)
        {
            Models.Template template = this._baseRepository.Get(templateId);

            // Example - {{ lead.name }}
            var model = new Models.Leads { Name = "john" };

            var templateParse = Template.Parse(template.Content);
            var templateRender = templateParse.Render(Hash.FromAnonymousObject(new
            {
                lead = model
            }));

            return templateRender;
        }
    }
}
