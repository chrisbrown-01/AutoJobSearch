using AngleSharp;
using AngleSharp.Dom;
using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System.Net.Http.Headers;

namespace AutoJobSearchConsoleApp
{
    internal class Program
    {
        // TODO: format (codemaid?)
        // TODO: email service and email formatter
        // TODO: simple UI/GUI
        // TODO: keep track of what job search query the listing was found in
        static async Task Main(string[] args)
        {
            //await SeleniumTesting.Execute();
            //await LocalFileTesting.CreateJsonFiles();
            //LocalFileTesting.LoadFromJsonFileTests();
            //LocalFileTesting.ScoringTest(); 
            //LocalFileTesting.CheckForDuplicatesTest();

            //FuzzyStringTesting.Test1();
            //FuzzyStringTesting.TestStringContains();

            //SQLiteTesting.CreateDb();
            //await SQLiteTesting.PopulateDb(await LocalFileTesting.GetJobListingsFromFiles());
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

        /* AngleSharp
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(JOBS_URL);
        var test = document.Prettify();
        */
    }
}