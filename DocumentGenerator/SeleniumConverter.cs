using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace DocumentGenerator
{
    public class SeleniumConverter : IConverterStrategy
    {

        public byte[] Convert(string document, string assetsUrl)
        {

            string tempfile = "temp-" + Random.Shared.Next(1000, 10000) + ".html";

            string tempPath = Path.Combine(new Uri(assetsUrl).AbsoluteUri + "/" , tempfile);
            try
            {
                var driveroptions = new ChromeOptions();
                driveroptions.AddArgument("--headless=new");
                File.WriteAllText(assetsUrl + "/" + tempfile, document);
                WebDriver driver = new ChromeDriver(driveroptions)
                {
                    Url = tempPath,
                };
                PrintOptions options = new PrintOptions();
                options.OutputBackgroundImages = true;
                options.PageDimensions = PrintOptions.PageSize.A4;
                Thread.Sleep(500);
                PrintDocument print = driver.Print(options);
                driver.Quit();
                return print.AsByteArray;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                File.Delete(assetsUrl + "/" + tempfile);
            }
        }
    }
}
