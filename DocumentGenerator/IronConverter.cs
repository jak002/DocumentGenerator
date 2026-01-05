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
            TextHeaderFooter header = new TextHeaderFooter
            {
                RightText = DateTime.Now.ToString()
            };

            TextHeaderFooter footer = new TextHeaderFooter
            {
                RightText = "Side {page} af {total-pages}",
            };
            _renderer.RenderingOptions.WaitFor.AllFontsLoaded();

            var pdf = _renderer.RenderHtmlAsPdf(document, assetsUrl);
            pdf.AddTextHeaders(header);
            pdf.AddTextFooters(footer);

            return pdf.BinaryData;
        }
    }
}
