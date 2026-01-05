using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weasyprint.Wrapped;

namespace DocumentGenerator
{
    public class WeasyConverter : IConverterStrategy
    {
        public WeasyConverter()
        {
            Task.Run(() => new Printer().Initialize()).GetAwaiter().GetResult();
        }
        public byte[] Convert(string document, string assetsUrl)
        {
            PrintResult result =  Task.Run(() => new Printer().Print(document, $"-u {new Uri(assetsUrl).AbsoluteUri + "/"}")).GetAwaiter().GetResult();
            if (result.ExitCode > 0)
            {
                throw new Exception($"Weasyprint finished with code {result.ExitCode}: {result.Error}");
            } else
            {
                return result.Bytes;
            }
        }
    }
}
