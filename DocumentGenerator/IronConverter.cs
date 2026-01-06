using IronPdf.Engines.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerator
{
    public class IronConverter : IConverterStrategy
    {
        private ChromePdfRenderer _renderer;
        public IronConverter()
        {
            IronPdf.License.LicenseKey = IronLicense.Key;
            _renderer = new ChromePdfRenderer();
        }

        public byte[] Convert(string document, string assetsUrl)
        {
            // Headers og footers skal oprettes separat fra dokument.
            TextHeaderFooter header = new TextHeaderFooter
            {
                RightText = DateTime.Now.ToString()
            };

            TextHeaderFooter footer = new TextHeaderFooter
            {
                RightText = "Side {page} af {total-pages}",
            };
            _renderer.RenderingOptions.WaitFor.AllFontsLoaded();

            //Dokument konverteres først...
            var pdf = _renderer.RenderHtmlAsPdf(document, assetsUrl);
            // Header og footer tilføjes bagefter.
            pdf.AddTextHeaders(header);
            pdf.AddTextFooters(footer);

            return pdf.BinaryData;
        }
    }
}
