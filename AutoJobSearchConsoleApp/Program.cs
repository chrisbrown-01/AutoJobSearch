using AngleSharp;
using AngleSharp.Dom;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Net.Http.Headers;

namespace AutoJobSearchConsoleApp
{
    internal class Program
    {
        private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs&start=0";
        //private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs";
        //private const string JOBS_URL = "https://www.google.com";
        //private const string JOBS_URL = "https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl";

        // start: final index of "Job Description"
        // end: index of "Show full description" or "Report this listing"
        static async Task Main(string[] args)
        {
            /*
            var doc = new HtmlDocument();
            doc.LoadHtml(TestItems.JobsViewSource);

            var ulElements = doc.DocumentNode.SelectNodes("//ul").ToList();
            var liElements = doc.DocumentNode.SelectNodes("//li").ToList();

            var innerTexts = new List<string>();

            foreach(var item in liElements)
            {
                innerTexts.Add(item.InnerText);
            }

            var innerTextsClean = new List<string>();

            foreach(var item in innerTexts)
            {

            }

            Console.WriteLine();
            */

            //var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArguments("headless");
            //var driver = new ChromeDriver(chromeOptions);

            var innerTexts = new List<string>();
            var linksList = new List<string>();
            var doc = new HtmlDocument();
            var driver = new ChromeDriver();

            driver.Navigate().GoToUrl(JOBS_URL);
            doc.LoadHtml(driver.PageSource);

            // foreach item in links, parse item.OuterHtml for "href" or Regex for website link
            //var links = doc.DocumentNode.Descendants("a")
            //               .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase))
            //               .ToList();

            var liElements = doc.DocumentNode.SelectNodes("//li").ToList();

            foreach (var li in liElements)
            {
                var links = li.Descendants("a")
                    .Where(a => a.InnerText.Contains("apply", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine();
            }

            // TODO: run once then save as json file for reuse later.
            //for(int i = 0; i < 101; i += 10)
            //{
            //    driver.Navigate().GoToUrl($"https://www.google.com/search?q=.net+jobs+tennessee&ibp=htl;jobs&start={i}");

            //    doc.LoadHtml(driver.PageSource);
            //    var liElements = doc.DocumentNode.SelectNodes("//li").ToList();
            //    foreach (var item in liElements)
            //    {
            //        innerTexts.Add(item.InnerText);
            //    }

            //    await Task.Delay(Random.Shared.Next(1000, 2500));
            //}

            Console.WriteLine();

            driver.Close();
            driver.Quit();
            
        }

        /*
        HttpClient _httpClient = new();
        var request = new HttpRequestMessage(HttpMethod.Get, JOBS_URL);
        var userAgentString = new ProductInfoHeaderValue("Chrome", "116.0.0.0");
        //var productValue = new ProductInfoHeaderValue("TestService", "1.0");
        //var commentValue = new ProductInfoHeaderValue("(+https://www.google.com)");
        request.Headers.UserAgent.Add(userAgentString);
        //request.Headers.UserAgent.Add(productValue);
        //request.Headers.UserAgent.Add(commentValue);
        var response = await _httpClient.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        _httpClient.Dispose();
        */

        /*
        HttpClient _httpClient = new();
        var result = await _httpClient.GetAsync(JOBS_URL);
        var test = await result.Content.ReadAsStringAsync();
        _httpClient.Dispose();
        */

        /*
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(JOBS_URL);
        var test = document.Prettify();
        */
}
}