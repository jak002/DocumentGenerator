// See https://aka.ms/new-console-template for more information
using DocumentGenerator;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection;

Console.WriteLine("Hello, World!");

ITemplaterStrategy cottle = new CottleTemplater();
ITemplaterStrategy handlebars = new HandlebarsTemplater();
ITemplaterStrategy razorlight = new RazorLightTemplater();



//RazorLightTest(razorlight);

//HandlebarsTest(templater);

string cottleTemplate = File.OpenText(@"faktura_cottle.html").ReadToEnd();
string handlebarsTemplate = File.OpenText(@"faktura_handlebars.html").ReadToEnd();
string razorLightTemplate = File.OpenText(@"faktura_razorlight.cshtml").ReadToEnd();

string bill = File.OpenText(@"faktura_convert.html").ReadToEnd();
string document = File.OpenText(@"document_convert.html").ReadToEnd();
string assetUrl = Environment.CurrentDirectory.ToString();
Dictionary<string, object> htmlObjects = new Dictionary<string, object>
{
    ["name"] = "Jacob Sejer Kristoffersen",
    ["date"] = DateTime.Now.ToString("d", new CultureInfo("da-DK")),
    ["paymentDue"] = new DateTime(2025, 12, 12).ToString("D", new CultureInfo("da-DK")),
    ["bills"] = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            ["name"] = "Vedligeholdelse af software",
            ["hours"] = 160,
            ["price"] = 200.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Udvikling & deployment support",
            ["hours"] = 50,
            ["price"] = 260.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Optræning af IT-personale",
            ["hours"] = 10,
            ["price"] = 250.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Fix printeren (igen)",
            ["hours"] = 1,
            ["price"] = 6000.00
        }
    },
    ["paidAutomatically"] = false
};

Dictionary<string, object> htmlObjectsLogicLess = ParseForHandlebars(htmlObjects);

WriteDocumentToDisk(bill, document, htmlObjectsLogicLess, assetUrl, new WeasyConverter(), "WeasyPrint");
WriteDocumentToDisk(bill, document, htmlObjectsLogicLess, assetUrl, new IronConverter(), "IronPDF");
WriteDocumentToDisk(bill, document, htmlObjectsLogicLess, assetUrl, new SeleniumConverter(), "Browser");
WriteDocumentToDisk(bill, document, htmlObjectsLogicLess, assetUrl, new PdfSharpConverter(), "PdfSharp");

//PerformancePdfTest(htmlObjectsLogicLess, bill, assetUrl);

//File.WriteAllText("faktura_razorlight_result.html", razorlight.GenerateDocument(razorLightTemplate, htmlObjects));

//File.WriteAllText("faktura_cottle_result.html", cottle.GenerateDocument(cottleTemplate, htmlObjects));
//File.WriteAllText("faktura_handlebars_result.html", handlebars.GenerateDocument(handlebarsTemplate, htmlObjectsLogicLess));

//PerformanceTest(cottle, handlebars, razorlight, cottleTemplate, handlebarsTemplate, razorLightTemplate, htmlObjects, htmlObjectsLogicLess, stopwatch);

static void HandlebarsTest(ITemplaterStrategy templater)
{
    string template = "I am testing {{library}} to see if it's {{adjective}} at templating.\r\n";

    template += "{{#each bills}}\r\n";
    template += "{{this.name}}: {{Math.Multiply this.price this.hours}}.00 kr\r\n";
    template += "{{/each}}";



    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
    {
        ["library"] = "Handlebars",
        ["adjective"] = "functional",
        ["bills"] = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            ["name"] = "Vedligeholdelse af software",
            ["hours"] = 160,
            ["price"] = 200.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Udvikling & deployment support",
            ["hours"] = 50,
            ["price"] = 260.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Optræning af IT-personale",
            ["hours"] = 10,
            ["price"] = 250.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Fix printeren (igen)",
            ["hours"] = 1,
            ["price"] = 6000.00
        }
    }
    };

    string result = templater.GenerateDocument(template, keyValuePairs);
    Console.WriteLine(result);
}

static void RazorLightTest(ITemplaterStrategy templater)
{
    string template = "I am testing @Model.library to see if it's @Model.adjective at templating.\r\n";

    template += "@foreach(var entry in Model.bills)\r\n{";
    template += "<text>@entry[\"name\"]: @entry[\"price\"] kr</text>\r\n";
    template += "}";



    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
    {
        ["library"] = "RazorLight",
        ["adjective"] = "functional",
        ["bills"] = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            ["name"] = "Vedligeholdelse af software",
            ["hours"] = 160,
            ["price"] = 200.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Udvikling & deployment support",
            ["hours"] = 50,
            ["price"] = 260.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Optræning af IT-personale",
            ["hours"] = 10,
            ["price"] = 250.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Fix printeren (igen)",
            ["hours"] = 1,
            ["price"] = 6000.00
        }
    }
    };

    string result = templater.GenerateDocument(template, keyValuePairs);
    Console.WriteLine(result);
}

static void CottleTest(ITemplaterStrategy templater)
{
    string template = "I am testing {library} to see if it's {adjective} at templating.\r\n";
    template += "{for item in bills:{item.name}: {format(item.hours*item.price, \"C\", \"da-DK\")} {set total to total+(item.hours*item.price)}\r\n}";
    template += "\r\n\r\n";
    template += "Total: {format(total, \"C\", \"da-DK\")}\r\n";
    template += "Moms: {format(total*0.25, \"C\", \"da-DK\")}";



    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
    {
        ["library"] = "Cottle",
        ["adjective"] = "functional",
        ["bills"] = new List<Dictionary<string, object>>
    {
        new Dictionary<string, object>
        {
            ["name"] = "Vedligeholdelse af software",
            ["hours"] = 160,
            ["price"] = 200.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Udvikling & deployment support",
            ["hours"] = 50,
            ["price"] = 260.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Optræning af IT-personale",
            ["hours"] = 10,
            ["price"] = 250.00
        },
        new Dictionary<string, object>
        {
            ["name"] = "Fix printeren (igen)",
            ["hours"] = 1,
            ["price"] = 6000.00
        }
    }
    };



    string result = templater.GenerateDocument(template, keyValuePairs);
    Console.WriteLine(result);
}

static void RunTemplate(ITemplaterStrategy templater, string template, Dictionary<string, object> htmlObjects, Stopwatch timer, List<string> measures)
{
    timer.Restart();
    string htmlResult = templater.GenerateDocument(template, htmlObjects);
    timer.Stop();
    measures.Add($"{templater.GetType().Name}: {timer.ElapsedTicks} ticks, {timer.ElapsedMilliseconds} ms");
}

static Dictionary<string, object> ParseForHandlebars(Dictionary<string, object> htmlObjects)
{
    Dictionary<string, object> htmlObjectsLogicLess = new Dictionary<string, object>();
    foreach (var kvp in htmlObjects)
    {
        htmlObjectsLogicLess[kvp.Key] = kvp.Value;
    }
    htmlObjectsLogicLess.Add("total", 0.0);
    foreach (var entry in (List<Dictionary<string, object>>)htmlObjectsLogicLess["bills"])
    {
        double entryTotal = (double)entry["price"] * (int)entry["hours"];
        double subtotal = (double)htmlObjectsLogicLess["total"] + entryTotal;
        htmlObjectsLogicLess["total"] = subtotal;
    }
    return htmlObjectsLogicLess;
}

static void PerformanceTest(ITemplaterStrategy cottle, ITemplaterStrategy handlebars, ITemplaterStrategy razorlight, string cottleTemplate, string handlebarsTemplate, string razorlightTemplate, Dictionary<string, object> htmlObjects, Dictionary<string, object> htmlObjectsLogicLess)
{
    Stopwatch stopwatch = new Stopwatch();
    List<string> measures = new List<string>();
    for (int i = 0; i <= 10; i++)
    {
        RunTemplate(cottle, cottleTemplate, htmlObjects, stopwatch, measures);
    }
    for (int i = 0; i <= 10; i++)
    {
        RunTemplate(handlebars, handlebarsTemplate, htmlObjectsLogicLess, stopwatch, measures);
    }
    for (int i = 0; i <= 10; i++)
    {
        RunTemplate(razorlight, razorlightTemplate, htmlObjects, stopwatch, measures);
    }

    foreach (var measure in measures)
    {
        Console.WriteLine(measure);
    }
}

static void PerformancePdfTest(Dictionary<string, object> htmlObjects, string template, string assets)
{
    // Mise en place.
    Stopwatch stopwatch = new Stopwatch();
    List<IConverterStrategy> converters = new List<IConverterStrategy>
    {
        new WeasyConverter(),
        new IronConverter(),
        new SeleniumConverter(),
        new PdfSharpConverter()
    };

    // Templater bliver opsat og “opvarmet” her.
    ITemplaterStrategy templater = new HandlebarsTemplater();
    templater.GenerateDocument(template, htmlObjects);
    DocumentGenerator.DocumentGenerator.SetTemplater(templater);

    // Test begynder her...
    Console.WriteLine("Initialized. Beginning test...");
    foreach (var converter in converters)
    {
        DocumentGenerator.DocumentGenerator.SetConverter(converter);
        Console.Write("Testing for " + converter.GetType() + ": ");

        // Cold start holdes separat.
        stopwatch.Restart();
        DocumentGenerator.DocumentGenerator.Generate(template, htmlObjects, assets);
        stopwatch.Stop();
        Console.WriteLine($"Cold start: {stopwatch.ElapsedTicks} ({stopwatch.ElapsedMilliseconds} ms)");

        // Batch test. Her genereres det samme document 10 gange, og gennemsnittet printes til sidst.
        List<long> measures = new List<long>();
        for (int i = 0; i < 10; i++)
        {
            stopwatch.Restart();
            DocumentGenerator.DocumentGenerator.Generate(template, htmlObjects, assets);
            stopwatch.Stop();
            Console.WriteLine($"Run {i + 1}: {stopwatch.ElapsedTicks}");
            measures.Add(stopwatch.ElapsedTicks);
        }

        long average = measures.Sum() / 10;
        Console.WriteLine($"Average: {average} ticks ({average / 10000.0} ms)");
    }
}

static void WriteDocumentToDisk(string bill, string document, Dictionary<string, object> htmlObjectsLogicLess, string assetUrl, IConverterStrategy converter, string outputName)
{
    DocumentGenerator.DocumentGenerator.SetConverter(converter);
    Console.WriteLine($"Generating bill with {outputName}...");
    byte[] resultbill = DocumentGenerator.DocumentGenerator.Generate(bill, htmlObjectsLogicLess, assetUrl);
    File.WriteAllBytes($"{outputName}Bill.pdf", resultbill);
    Console.WriteLine($"Bill generated. Generating document with {outputName}...");
    byte[] result = DocumentGenerator.DocumentGenerator.Generate(document, htmlObjectsLogicLess, assetUrl);
    File.WriteAllBytes($"{outputName}Document.pdf", result);
    Console.WriteLine($"All documents successfully generated with {outputName}!");
}