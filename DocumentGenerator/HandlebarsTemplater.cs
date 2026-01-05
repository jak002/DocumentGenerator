using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerator
{
    public class HandlebarsTemplater : ITemplaterStrategy
    {
        private IHandlebars handlebarsContext = Handlebars.Create();
        private string priortemplate;
        HandlebarsTemplate<object, object> currentTemplate;

        public HandlebarsTemplater()
        {
            handlebarsContext.Configuration.ThrowOnUnresolvedBindingExpression = true;
            HandlebarsHelpers.Register(handlebarsContext);
        }
        public string GenerateDocument(string template, Dictionary<string, object> values)
        {
            if (template != priortemplate)
            {
                currentTemplate = handlebarsContext.Compile(template);
                priortemplate = template;
            }
            return currentTemplate(values);
        }
    }
}
