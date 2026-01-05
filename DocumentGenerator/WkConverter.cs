using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using System.Runtime.InteropServices;

namespace DocumentGenerator
{
    public class WkConverter : IConverterStrategy
    {
        private IConverter? converter;
        private static bool _nativeLoaded = false;
        private static readonly object _lock = new();

        private static void EnsureLoaded()
        {
            if (_nativeLoaded) return;

            lock (_lock)
            {
                if (_nativeLoaded) return;

                string arch = Environment.Is64BitOperatingSystem ? "win-x64" : "win-x86";

                string dllpath = Path.Combine(
                    AppContext.BaseDirectory,
                    "runtimes",
                    arch,
                    "native",
                    RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "wkhtmltox.dll" : "wkhtmltox.so"
                    );

                if (!File.Exists(dllpath))
                    throw new FileNotFoundException($"Found no WKHTML library at {dllpath}");

                NativeLibrary.Load(dllpath);

                _nativeLoaded = true;
            }
        }

        public byte[] Convert(string document, string assetsUrl)
        {

            EnsureLoaded();

            converter ??= new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    //Orientation = Orientation.Portrait,
                    //Margins = new MarginSettings() {Top = 15, Bottom = 15, Left = 10, Right = 10},
                    //Out = @"WkhtmlResult.pdf",
                    
                    
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        //PagesCount = true,
                        //Encoding = Encoding.UTF8,
                        //LoadSettings = new LoadSettings()
                        //{
                            //BlockLocalFileAccess = false,
                        //},
                        //WebSettings = {UserStyleSheet = assetsUrl + "/style.css"},
                        //HtmlContent = document,
                        HtmlContent = "<p>test</p>"
                        //HeaderSettings = new HeaderSettings() {FontSize = 9, Right = "[date] [time]", Spacing = 10},
                        //FooterSettings = new FooterSettings() {FontSize = 9, Right = "Side [page]/[toPage]"}
                    }
                }
            };

            return converter.Convert(doc);
        }
    }
}
