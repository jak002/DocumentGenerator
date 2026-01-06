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
            // Filnavn genereres tilfældigt, gør overskrivelse mindre sansylig.
            string tempfile = "temp-" + Random.Shared.Next(1000, 10000) + ".html";

            // Filsti genereres, i file:// format.
            string tempPath = Path.Combine(new Uri(assetsUrl).AbsoluteUri + "/" , tempfile);
            try
            {
                var driveroptions = new ChromeOptions();
                // Browser køres headless for at spare cpu.
                driveroptions.AddArgument("--headless=new");
                // HTML fil laves, opbygget på samme måde som tempPath (dog uden file://).
                File.WriteAllText(assetsUrl + "/" + tempfile, document);
                // Åbn html fil i browser. CSS og assets burde virke automatisk grundet filsti.
                WebDriver driver = new ChromeDriver(driveroptions)
                {
                    Url = tempPath,
                };
                PrintOptions options = new PrintOptions();
                options.OutputBackgroundImages = true;
                options.PageDimensions = PrintOptions.PageSize.A4;
                // Vent på at ekstern skrifttype loades...
                Thread.Sleep(500);
                // Og print herefter.
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
                // Oprydning skal helst ske uanset om der var fejl.
                File.Delete(assetsUrl + "/" + tempfile);
            }
        }
    }
}
