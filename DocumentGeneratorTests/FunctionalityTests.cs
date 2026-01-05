using Microsoft.VisualStudio.TestTools.UnitTesting;
using DocumentGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace DocumentGenerator.Tests
{
    [TestClass()]
    public class FunctionalityTests
    {
        ITemplaterStrategy Cottle = new CottleTemplater();
        ITemplaterStrategy Handlebars = new HandlebarsTemplater();
        ITemplaterStrategy RazorLight = new RazorLightTemplater();

        private ITemplaterStrategy SetTemplater(string templater)
        {
            switch (templater)
            {
                case "Cottle":
                    return Cottle;
                case "Handlebars":
                    return Handlebars;
                case "RazorLight":
                    return RazorLight;
                default:
                    throw new Exception("No templater set");
            }
        }

        [DataTestMethod()]
        [DataRow("Cottle", "This is test number {number}")]
        [DataRow("Handlebars", "This is test number {{number}}")]
        [DataRow("RazorLight", "This is test number @Model.number")]
        public void ReplaceNumericData(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["number"] = 1 });

            Assert.AreEqual("This is test number 1", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "This test is {status}!")]
        [DataRow("Handlebars", "This test is {{status}}!")]
        [DataRow("RazorLight", "This test is @Model.status!")]
        public void ReplaceStringData(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["status"] = "successful" });

            Assert.AreEqual("This test is successful!", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "{number1} plus {number2} is {number1+number2}")]
        [DataRow("Handlebars", "{{number1}} plus {{number2}} is {{Math.Add number1 number2}}")]
        [DataRow("RazorLight", "@Model.number1 plus @Model.number2 is @(Model.number1 + Model.number2)")]
        public void HandleMath(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["number1"] = 2, ["number2"] = 5 });

            Assert.AreEqual("2 plus 5 is 7", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "Your total is {format(price, \"C\")}")]
        [DataRow("Handlebars", "Your total is {{String.Format price \"C\"}}")]
        [DataRow("RazorLight", "Your total is @Model.price.ToString(\"C\")")]
        public void ConvertToCurrency(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["price"] = 1700.00 });

            Assert.AreEqual("Your total is 1.700,00 kr.", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "Your total is {format(price*times, \"C\")}")]
        [DataRow("Handlebars", "Your total is {{String.Format (Math.Multiply price times) \"C\"}}")]
        [DataRow("RazorLight", "Your total is @((Model.price*Model.times).ToString(\"C\"))")]
        public void NestedLogic(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["price"] = 1700.00, ["times"] = 5 });

            Assert.AreEqual("Your total is 8.500,00 kr.", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "And a {list[0]} and a {list[1]} and a{for entry in list: {entry}}")]
        [DataRow("Handlebars", "And a {{list.[0]}} and a {{list.[1]}} and a{{#each list}} {{this}}{{/each}}")]
        [DataRow("RazorLight", "And a @Model.list[0] and a @Model.list[1] and a@{foreach (int number in Model.list){<Text> @number</Text>}}")]
        public void NumberedCollections(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            Dictionary<string, object> values = new Dictionary<string, object>();
            List<int> collection = new List<int>();
            for (int i = 1; i <= 4; i++)
            {
                collection.Add(i);
            }
            values.Add("list", collection);
            string result = Templater.GenerateDocument(template, values);

            Assert.AreEqual("And a 1 and a 2 and a 1 2 3 4", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "And a {list[\"1\"]} and a {list[\"2\"]} and a{for entry in list: {entry}}")]
        [DataRow("Handlebars", "And a {{list.1}} and a {{list.2}} and a{{#each list}} {{this}}{{/each}}")]
        [DataRow("RazorLight", "And a @Model.list[\"1\"] and a @Model.list[\"2\"] and a@{foreach (string number in Model.list.Values){<Text> @number</Text>}}")]
        public void KeyedCollections(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            Dictionary<string, object> values = new Dictionary<string, object>();
            Dictionary<string, string> collection = new Dictionary<string, string>();
            for (int i = 1; i <= 4; i++)
            {
                collection.Add(i.ToString(), i + "!");
            }
            values.Add("list", collection);
            string result = Templater.GenerateDocument(template, values);

            Assert.AreEqual("And a 1! and a 2! and a 1! 2! 3! 4!", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "This statement {if first:is|else:isn't} true. This other one is{if !second:n't}")]
        [DataRow("Handlebars", "This statement {{#if first}}is{{else}}isn't{{/if}} true. This other one is{{#unless second}}n't{{/unless}}")]
        [DataRow("RazorLight", "This statement @{if (Model.first){<Text>is</text>;} else{<Text>isn't</Text>;}} true. This other one is@{if(!Model.second){<Text>n't</Text>}}")]
        public void IfElse(string templater, string template)
        {
            ITemplaterStrategy Templater = SetTemplater(templater);
            string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["first"] = true, ["second"] = false });
            Assert.AreEqual("This statement is true. This other one isn't", result);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "Calling unknown value: {value}")]
        [DataRow("Handlebars", "Calling unknown value: {{value}}")]
        [DataRow("RazorLight", "Calling unknown value: @Model.value")]
        public void ErrorHandlingData(string templater, string template)
        {
            int errors = 0;
            ITemplaterStrategy Templater = SetTemplater(templater);
            try
            {
                string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["unexpectedValue"] = "Oh no" });
            }
            catch (Exception)
            {
                errors++;
            }
            Assert.AreNotEqual(0, errors);
        }

        [DataTestMethod()]
        [DataRow("Cottle", "Oh no I misspelled something: {value")]
        [DataRow("Handlebars", "Oh no I misspelled something: {{value}")]
        [DataRow("RazorLight", "Oh no I misspelled something: @Modle.value")]
        public void ErrorHandlingTemplate(string templater, string template)
        {
            int errors = 0;
            ITemplaterStrategy Templater = SetTemplater(templater);
            try
            {
                string result = Templater.GenerateDocument(template, new Dictionary<string, object> { ["value"] = "text" });
            }
            catch (Exception)
            {
                errors++;
            }
            Assert.AreNotEqual(0, errors);
        }
    }
}