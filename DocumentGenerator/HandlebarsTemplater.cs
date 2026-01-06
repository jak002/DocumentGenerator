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
        // Ny context laves i stedet for at bruge global, for konfigurering og helpers.
        private IHandlebars handlebarsContext = Handlebars.Create();
        private string priortemplate;
        HandlebarsTemplate<object, object> currentTemplate;

        public HandlebarsTemplater()
        {
            // Handlebars ignorerer normalt manglende datafelter. Ved dette kald bliver der lavet exceptions i stedet.
            handlebarsContext.Configuration.ThrowOnUnresolvedBindingExpression = true;
            // Helper metoder til logik.
            HandlebarsHelpers.Register(handlebarsContext);
        }
        public string GenerateDocument(string template, Dictionary<string, object> values)
        {
            // Check om cachet...
            if (template != priortemplate)
            {
                currentTemplate = handlebarsContext.Compile(template);
                priortemplate = template;
            }
            return currentTemplate(values);
        }
    }
}
