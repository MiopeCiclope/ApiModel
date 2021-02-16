using System;
using System.Globalization;
using System.Linq;
using DotLiquid;
using KivalitaAPI.Common;

namespace KivalitaAPI.Services
{
    public static class TextFilters
    {

        public static string AddDays(string input, int days = 0)
        {
            string dateFormat = "dd/MM/yyyy";
            DateTime date;

            if (input.ToLower() == "now")
            {
                date = DateTime.Now;
            }
            else if (DateUtils.IsDate(input))
            {
                date = DateTime.Parse(input);
            }
            else
            {
                return "";
            }

            date = date.AddDays(days);
            date = DateUtils.GetBusinessDate(date);

            return date.ToString(dateFormat);
        }
    }

    public class TemplateTransformService
    {
        public TemplateTransformService() { }

        public string TransformLead(string text, Models.Leads leadData)
        {

            Template.RegisterFilter(typeof(TextFilters));

            RegisterSafeTypes();

            var templateParse = Template.Parse(text);

            var templateRender = templateParse.Render(Hash.FromAnonymousObject(new
            {
                leads = leadData,
                company = leadData.Company
            }));

            return templateRender;
        }
        public string Transform(string text, Models.Leads leadData)
        {

            Template.RegisterFilter(typeof(TextFilters));

            RegisterSafeTypes();

            var templateParse = Template.Parse(text);

            var templateRender = templateParse.Render(Hash.FromAnonymousObject(new
            {
                leads = leadData,
                company = leadData.Company
            }));

            return templateRender;
        }

        public bool IsValid(string text)
        {
            try
            {
                var templateParse = Template.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void RegisterSafeTypes()
        {
            Template.RegisterSafeType(typeof(Models.Leads),
                typeof(Models.Leads).GetProperties().Select(x => x.Name).ToArray());

            Template.RegisterSafeType(typeof(Models.Company),
                typeof(Models.Company).GetProperties().Select(x => x.Name).ToArray());
        }
    }
}
