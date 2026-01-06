using Microsoft.AspNetCore.Components.Web;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace DocumentGenerator
{
    public class PdfSharpConverter : IConverterStrategy
    {
        public byte[] Convert(string document, string assetsUrl)
        {
            // Manuel ombygning fra relative stier til absolutte stier. Det er ikke pænt men PDFSharp håndterer dem ellers ikke.
            string formatedDocument = document.Replace("<img src=\"", "<img src=\"" + assetsUrl + "/").Replace("rel=\"stylesheet\" href=\"", "rel=\"stylesheet\" href=\"" + assetsUrl + "/");
            // Encoding fra desktop .NET, for PdfGenerator.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // Pdf dokument genereres, i a4 størrelse.
            PdfSharp.Pdf.PdfDocument pdf = PdfGenerator.GeneratePdf(formatedDocument, PdfSharp.PageSize.A4);
            // MemoryStream behøves da pdf ikke kan returnere byte array...
            MemoryStream stream = new MemoryStream();
            // Så pdf fil gemmes til stream, som herefter returnerers som array.
            pdf.Save(stream, false);
            return stream.ToArray();
        }
    }
}
