using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerator
{
    public static class DocumentGenerator
    {
        private static ITemplaterStrategy? _templater;
        private static IConverterStrategy? _pdfConverter;

        /// <summary>
        /// Udskifter templater med den videregivne, til brug i Generate metoden.
        /// </summary>
        /// <param name="templater"></param>
        public static void SetTemplater(ITemplaterStrategy templater)
        {
            _templater = templater;
        }

        /// <summary>
        /// Udskifter converter med den videregivne, til brug i Generate metoden.
        /// </summary>
        /// <param name="converter"></param>
        public static void SetConverter(IConverterStrategy converter)
        {
            _pdfConverter = converter;
        }

        /// <summary>
        /// Kalder templaterens "GenerateDocument" metode og conveterens "Convert" metode.
        /// </summary>
        /// <param name="template">HTML-skabelon, lavet til den templater der er sat (standard: handlebars)</param>
        /// <param name="values">Dictionary af dynamisk data til indsættelse i HTML-skabelonen</param>
        /// <param name="assetsUrl">Absolut filsti til assets nævnt i skabelon</param>
        /// <returns></returns>
        public static byte[] Generate(string template, Dictionary<string, object> values, string assetsUrl)
        {
            // Handlebars sat som standard jf. rapporten.
            if (_templater == null)
            {
                _templater = new HandlebarsTemplater();
            }
            // Selenium sat som standard jf. rapporten.
            if (_pdfConverter == null)
            {
                _pdfConverter = new SeleniumConverter();
            }
            try
            {
                string document = _templater.GenerateDocument(template, values);
                return _pdfConverter.Convert(document, assetsUrl);
            } catch (Exception)
            {
                throw;
            }

        }
    }
}
