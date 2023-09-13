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
            await SQLiteTesting.PopulateDb(LocalFileTesting.LoadFromJsonFile(Paths.MULTI_PAGE_JSON_FILE_PATH));
            //await SQLiteTesting.GetAllLinks();
            //await SQLiteTesting.UpdateJobListing();
        }
    }
}