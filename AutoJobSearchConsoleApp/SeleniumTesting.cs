using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class SeleniumTesting
    {
        private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs&start=0";
        private const int MAX_START_PAGE = 100;

        public static async Task Execute()
        {
            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");
            //var driver = new ChromeDriver(chromeOptions);

            var searchTerms = new List<string>()
            {
                ".net jobs tennessee",
                ".net jobs USA",
                "c# programmer USA",
                "c# programmer tennessee",
                "c# jobs in toronto"
            };

            var innerTexts = new List<string>();
            var linksList = new List<string>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            foreach (var searchTerm in searchTerms)
            {
                for (int i = 0; i < MAX_START_PAGE + 1; i += 10)
                {
                    driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start={i}");

                    doc.LoadHtml(driver.PageSource);
                    var liElements = doc.DocumentNode?.SelectNodes("//li")?.ToList();

                    if (liElements == null) break;

                    foreach (var item in liElements)
                    {
                        innerTexts.Add(item.InnerText);
                    }

                    await Task.Delay(Random.Shared.Next(3000, 6000));
                }
            }

            driver.Close();
            driver.Quit();
        }
    }
}
