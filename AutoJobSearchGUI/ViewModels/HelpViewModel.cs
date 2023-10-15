using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class HelpViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public static string AboutThisProject => "This project was built in October 2023 as a tool to help automate my job search. " +
            "This user interface was built with AvaloniaUI. The web scraping is accomplished using Selenium. Both the GUI and the " +
            "web scraper read/save data to a shared SQLite database. All code is written in C# and XAML.";

        public static string AboutJobBoard => "The job board tab displays the scraped job listings in a table format. Select a job and press the " +
            "\"Open Selected Job\" button to view the full job description and weblinks to apply at. The user can resize the width of columns " +
            "and sort the displayed job listings by clicking the column headers. Use the \"Previous/Next Page\" buttons to navigate through the" +
            "job listings. Query the database for specific results using the \"Advanced Query\" menu and selecting/enabling the appropriate filters. " +
            "The user can hide jobs by right clicking on a job listing and selecting \"Hide Job\". By default, the GUI will not display any jobs that were" +
            "hidden unless they use the Advanced Query menu.";

        public static string AboutJobSearch => "The job search tab allows the user to specify what jobs they wish to search for, as well as any keywords and " +
            "sentiments they wish to define for scoring the scraped jobs. To specify multiple search terms/keywords/sentiments, place each item on a " +
            "newline and end each line with a comma (ex. the first line should look like \"Job To Search For #1,\". Therefore do not include commas anywhere " +
            "except to define the end of a search term/keyword/sentiment. The user can start the job scraper by pressing the \"Execute Job Search\" button. " +
            "Multiple search profiles can be created by the user if you wish to split up your job searches and scoring keywords/sentiments.";

        public static string AboutJobScoring => "Job scoring is accomplished using the keywords and sentiments specified by the user in the job search profile " +
            "section. The keywords and sentiments are not case sensitive. " +
            "The score applied to the job increases by 1 everytime the job description contains a positive keyword, and decreases by 1 everytime the description " +
            "contains a negative keyword. The score increases by 1 everytime the description exceeds a similarity threshold based on how similar it is to each of the " +
            "positive sentiments, and likewise for negative sentiments. Sentiments should be atleast 1 sentence long but should not be longer than a few sentences long. " +
            "The sentiment scoring is accomplished using fuzzy string matching with the FuzzySharp library.";

        public static string AboutJobScraper => "The job scraper uses Selenium to open a Google Chrome browser and navigate to the Google jobs page. " +
            "Selenium will then search for jobs using the seach terms " +
            "specified by the user in the selected job search profile. The job scraper will automatically page through all of the job listings and in general" +
            "does not require any interaction from the user. The job scraper will automatically close the Google Chrome browser upon completion of searching " +
            "for jobs. Sometimes Google will determine that an automated program is using their platform and will throw captchas at the Selenium Chrome browser. " +
            "The application will detect if a captcha was displayed and will pause the job scraper until the user solves the captcha. If you encounter a captcha, " +
            "simply solve the captcha so the Selenium Chrome browser does display the Google jobs search page. Then enter \"CONTINUE\" into the console and the" +
            "web scraper will resume without any need for further user interaction. In my testing, Google blocks any sort of request to their job search " +
            "page if it isn't being made by a browser. Therefore you cannot use a headless browser or pure HTTP requests to scrape jobs.";
    }
}
