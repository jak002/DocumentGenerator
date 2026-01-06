using Microsoft.CodeAnalysis;
using RazorLight;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace DocumentGenerator
{
    public class RazorLightTemplater : ITemplaterStrategy
    {
        // Caching konfigureres her. Denne opsætning er mere eller mindre "ingen opsætning", hvor templateren bruger sig selv som caching resource.
        private RazorLightEngine engine = new RazorLightEngineBuilder()
            .UseEmbeddedResourcesProject(typeof(RazorLightTemplater).Assembly)
            .SetOperatingAssembly(typeof(RazorLightTemplater).Assembly)
            .UseMemoryCachingProvider()
            .Build();

        public string GenerateDocument(string template, Dictionary<string, object> values)
        {
            // Dictionary omdannes til objekt med alle values som properties.
            var valueObject = new ExpandoObject();
            // Hver entry i dictionary tilføjes bagefter.
            foreach (var kvp in values)
            {
                valueObject.TryAdd(kvp.Key, kvp.Value);
            }
            // Check cache...
            var cacheResult = engine.Handler.Cache.RetrieveTemplate("templateKey");
            if (cacheResult.Success)
            {
                var templatePage = cacheResult.Template.TemplatePageFactory();
                return Task.Run(async () => await engine.RenderTemplateAsync(templatePage, valueObject))
                    .GetAwaiter()
                    .GetResult();
            }
            // Metode cacher automatisk under den givne cache key.
            return Task.Run(async () => await engine.CompileRenderStringAsync("templateKey", template, valueObject))
                .GetAwaiter()
                .GetResult(); 
        }
    }
}
