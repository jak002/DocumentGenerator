# Data-to-PDF generator (via HTML)

This project was built as part of my dissertation, in an attempt to use open-source .NET components to build a package library that:
- Builds an HTML document from predetermined templates and object data, given as a dictionary of various types (numbers, strings, booleans, even enumerables)
- Then converts this HTML document (along with a filepath to any necessary assets, like styling, images etc) into a PDF.

The objective has been to build a minimal component, meant to be expanded based on individual projects' needs. The comoponent is split up in 2 parts: HTML-templating and PDF-conversion. These two subcomponents can be switched out dynamically, and more implementations can be added through the given interfaces.

## DocumentGenerator
This is the main component. Copy this over (or parts of it) or add a reference to this library to use it. Included in this library is the main static class (DocumentGenerator.cs), along with:

**Templating implementations:**
- *Handlebars-Net:* This is the default HTML templater. See https://github.com/Handlebars-Net/Handlebars.Net for documentation.
- *Cottle:* Faster and more lightweight than Handlebars, and can resolve logic in its templates/data. Doesn't include the same amount of error validation though. See https://github.com/r3c/cottle for documentation.
- *RazorLight:* Templating implementation of Microsoft's Razor syntax. Bear in mind that this requires more explicit configuration, see https://github.com/toddams/RazorLight for documentation.

**PDF-conversion implementations:**
- *Selenium:* This is the default converter, using an automated chrome browser to handle conversion. I consider this to be a healthy middle-ground between performance, compatibility, and functionality. Keep in mind that it is rather opinionated on relative filepaths for assets.
- *WeasyPrint:* Provides more functionality in terms of PDF standards. Keep in mind that this only works on Windows and Linux though, and requires explicit copying of its python engine (standalone-linux-64, standalone-windows-64, version-7.0.1), either through project properties or by hand.
It is also remarkably slower than Selenium and uses its own CLI syntax. See https://doc.courtbouillon.org/weasyprint/stable/index.html for documentation.
- *IronPDF:* The trial license on this is expired, so do with it what you please.
- *HtmlRenderer/PDFSharp:* Supports way less CSS, as it's a bit of a hacked together implementation of a package not intended to render HTML files. Don't expect results to reflect your HTML styling. It is blazingly fast though.
- *WkHtmlToPdf:* Doesn't work.

This component is intended to be used through the **Generate** method in the main class. Without any setup, this simply uses defaults for templating and conversion. If you wish to use other implementations, these are passed through **SetTemplater** and **SetConverter** methods. The resulting byte array can be saved to file, sent elsewhere, done whatever you want with.

Along with this, this solution also contains functionality tests (DocumentGeneratorTests) and a performance test/console implementation (GeneratorConsole). These are not necessary to copy over, but you can peruse them for ideas on how to use this component if you want.
