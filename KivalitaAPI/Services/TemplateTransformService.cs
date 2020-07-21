using System.Linq;
using DotLiquid;

namespace KivalitaAPI.Services
{
    public class TemplateTransformService
    {
        public TemplateTransformService() { }

        public string TransformLead(string text, Models.Leads leadData)
        {
            Template.RegisterSafeType(typeof(Models.Leads),
                typeof(Models.Leads).GetProperties().Select(x => x.Name).ToArray());

            var templateParse = Template.Parse(text);
            var templateRender = templateParse.Render(Hash.FromAnonymousObject(new
            {
                leads = leadData
            }));

            return templateRender;
        }
    }
}
