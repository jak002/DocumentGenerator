
using Cottle;
using Cottle.Exceptions;
using System.Collections;

namespace DocumentGenerator
{
    public class CottleTemplater : ITemplaterStrategy
    {
        string priortemplate;
        IDocument? currentTemplate;

        private Dictionary<Value, Value> ParseDictionaryToCottle(Dictionary<string, object> originalDict)
        {
            Dictionary<Value, Value> cottleDict = new Dictionary<Value, Value>();
            foreach (var entry in originalDict)
            {
                // Vi kan antage at keys altid kan læses som strings.
                string key = entry.Key;
                // Value kan være alt fra simple typer til enumerables, så parsing behøves.
                Value value = NativeTypeToCottleValue(entry.Value);
                cottleDict.Add(key, value);
            }
            return cottleDict;
        }

        private Value NativeTypeToCottleValue(object native)
        {
            switch (native)
            {
                // Simple typer bliver omdannet automatisk.
                case Value v: return v;
                case string s: return s;
                case double d: return d;
                case int i: return i;
                case bool b: return b;
                // Cottle har indbyggede metoder til omdannelse af funktioner.
                case IFunction f: return Value.FromFunction(f);
                case IDictionary dict:
                    Dictionary<Value, Value> valueDict = new Dictionary<Value, Value>();
                    foreach (var entry in dict.Keys)
                    {
                        // Key vil altid forventes at være en string.
                        Value key = Value.FromString((string?)entry);
                        // Value type kendes ikke - skal derfor igennem samme switch for at blive Value.
                        Value value = dict[entry] != null ? NativeTypeToCottleValue(dict[entry]) : Value.Undefined;
                        valueDict.Add(key, value);
                    }
                    return valueDict;
                case IEnumerable list:
                    List<Value> valueList = new List<Value>();
                    foreach (var i in list)
                    {
                        // Type kendes ikke - skal derfor igennem samme switch for at blive Value.
                        valueList.Add(NativeTypeToCottleValue(i));
                    }
                    return valueList;
                // Typer ikke i denne liste bliver set som fejl.
                default:
                    throw new InvalidCastException($"Could not parse {native.ToString} of type {native.GetType}");
            }
        }

        public string GenerateDocument(string template, Dictionary<string, object> values)
        {
            try
            {
                Dictionary<Value, Value> dict = ParseDictionaryToCottle(values);
                // Check om template er cachet...
                if (template != priortemplate)
                {
                    var documentResult = Document.CreateDefault(template);
                    currentTemplate = documentResult.DocumentOrThrow;
                    // Sæt ny template i cache.
                    priortemplate = template;
                }
                var context = Context.CreateBuiltin(dict);
                return currentTemplate.Render(context);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}