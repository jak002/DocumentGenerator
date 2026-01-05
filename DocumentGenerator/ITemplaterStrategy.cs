using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentGenerator
{
    public interface ITemplaterStrategy
    {
        public string GenerateDocument(string template, Dictionary<string, object> values);
    }
}
