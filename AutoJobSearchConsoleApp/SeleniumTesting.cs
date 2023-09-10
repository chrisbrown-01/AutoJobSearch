using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class SeleniumTesting
    {
        private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs&start=0";

        public static async Task Execute()
        {
            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");
            //var driver = new ChromeDriver(chromeOptions);

            var innerTexts = new List<string>();
            var linksList = new List<string>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            driver.Navigate().GoToUrl(JOBS_URL);
            doc.LoadHtml(driver.PageSource);

            // TODO: experiment with detecting the end of the Google list (in browser) or error handling if query start index becomes invalid
            for (int i = 0; i < 101; i += 10)
            {
                driver.Navigate().GoToUrl($"https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs&start={i}");

                var source = driver.PageSource;

                //doc.LoadHtml(driver.PageSource);
                //var liElements = doc.DocumentNode.SelectNodes("//li").ToList();
                //foreach (var item in liElements)
                //{
                //    innerTexts.Add(item.Description_Raw);
                //}

                await Task.Delay(Random.Shared.Next(1000, 2500));
            }

            Console.WriteLine();

            driver.Close();
            driver.Quit();


            await Task.CompletedTask;
        }
    }
}
