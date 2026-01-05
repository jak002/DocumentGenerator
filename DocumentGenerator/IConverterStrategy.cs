using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerator
{
    public interface IConverterStrategy
    {
        public byte[] Convert(string document, string assetsUrl);
    }
}
