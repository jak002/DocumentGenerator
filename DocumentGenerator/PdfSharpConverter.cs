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
            string formatedDocument = document.Replace("<img src=\"", "<img src=\"" + assetsUrl + "/").Replace("rel=\"stylesheet\" href=\"", "rel=\"stylesheet\" href=\"" + assetsUrl + "/");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            PdfSharp.Pdf.PdfDocument pdf = PdfGenerator.GeneratePdf(formatedDocument, PdfSharp.PageSize.A4);
            PdfPages pages = pdf.Pages;
            MemoryStream stream = new MemoryStream();
            pdf.Save(stream, false);
            return stream.ToArray();
        }
    }
}
