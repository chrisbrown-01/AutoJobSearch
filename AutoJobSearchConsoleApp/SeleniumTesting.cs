using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AutoJobSearchConsoleApp
{
    


    public class SeleniumTesting
    {
        private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&oq=.net+j&aqs=edge.1.69i57j69i59l2j69i60l2j69i61j69i65l2.2826j0j1&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start=100#htivrt=jobs&htidocid=uW2IE3St57iokg2KAAAAAA%3D%3D&fpstate=tldetail";
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
                //driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start=10");

                //string url = driver.Url; // Get updated URL that Google assigns, required to avoid captchas
                //var uriBuilder = new UriBuilder(url);
                //var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                for (int i = 0; i < MAX_START_PAGE + 1; i += 10)
                {
                    driver.Navigate().GoToUrl($"https://www.google.com/search?q={WebUtility.UrlEncode(searchTerm)}&sourceid=chrome&ie=UTF-8&ibp=htl;jobs&start={i}");

                    //query.Set("start", i.ToString());
                    //uriBuilder.Query = query.ToString();
                    //// uriBuilder.ToString()
                    //driver.Navigate().GoToUrl(uriBuilder.ToString());

                    doc.LoadHtml(driver.PageSource);

                    var checkForCaptcha = doc?.DocumentNode?.InnerText;

                    if(checkForCaptcha != null && checkForCaptcha.Contains("detected unusual traffic", StringComparison.OrdinalIgnoreCase))
                    {
                        var input = "";

                        while(input != "CONTINUE")
                        {
                            Console.WriteLine("Solve the Google Chrome captcha then type in 'CONTINUE' to continue scraping: ");
                            input = Console.ReadLine();
                        }

                        doc!.LoadHtml(driver.PageSource);
                    }     

                    var liElements = doc!.DocumentNode?.SelectNodes("//li")?.ToList();

                    if (liElements == null) break;

                    foreach (var item in liElements)
                    {
                        innerTexts.Add(item.InnerText);
                    }

                    //await Task.Delay(Random.Shared.Next(3000, 5000));
                }
            }

            driver.Close();
            driver.Quit();
        }
    }
}
