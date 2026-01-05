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

        public static void SetTemplater(ITemplaterStrategy templater)
        {
            _templater = templater;
        }

        public static void SetConverter(IConverterStrategy converter)
        {
            _pdfConverter = converter;
        }

        public static byte[] Generate(string template, Dictionary<string, object> values, string assetsUrl)
        {
            if (_templater == null)
            {
                _templater = new HandlebarsTemplater();
            }

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
