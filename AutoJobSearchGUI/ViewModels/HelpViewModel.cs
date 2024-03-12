namespace AutoJobSearchGUI.ViewModels
{
    // TODO: update help contents
    public partial class HelpViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public static string AboutJobBoard => "The Job Board tab displays the scraped job listings in a table format. Select a job and press the " +
            "\"Open Selected Job\" button to view the full job description and weblinks to apply at.\r\n\r\n" +
            "After executing the job scraper, you will need to use the Advanced Query \"Clear All Filters\" button or the Options \"Go To Default View\" " +
            "button to see the new jobs. Note that it may take a couple of minutes before the new jobs are available for display.\r\n\r\n" +
            "The user can resize the width of columns " +
            "and sort the displayed job listings by clicking the column headers. Use the \"Previous/Next Page\" buttons to navigate through the " +
            "job listings.\r\n\r\n" +
            "Query the database for specific results using the \"Advanced Query\" menu and enabling the appropriate filters.\r\n\r\n" +
            "The user can hide jobs by right clicking on a job listing and selecting \"Hide Job\". By default, the GUI will not display any jobs that were " +
            "hidden unless they use the Advanced Query option.";

        public static string AboutJobScoring => "Job scoring is accomplished using the keywords and sentiments specified by the user in the job search profile " +
            "section. The keywords and sentiments are not case sensitive.\r\n\r\n" +
            "The score applied to the job increases by 1 every time the job description contains a positive keyword, and decreases by 1 every time " +
            "the description contains a negative keyword.\r\n\r\n" +
            "The score increases by 1 every time the job description exceeds a similarity threshold based on how similar it is to each of the " +
            "positive sentiments, and likewise for negative sentiments.\r\n\r\n" +
            "Sentiments should be atleast 1 sentence long but should not be longer than a few sentences long.\r\n\r\n" +
            "Sentiment scoring is accomplished using fuzzy string matching with the FuzzySharp library.\r\n\r\n" +
            "If you wish to change the scoring thresholds used by the FuzzySharp library, you can update the \"FUZZ_RATIO_THRESHOLD\" properties in the " +
            "appsettings.json file found in the AutoJobSearchJobScraper directory.";

        public static string AboutJobScraper => "The Job Scraper uses Selenium to open a Google Chrome browser and navigate to the Google jobs page. " +
            "Selenium will then search for jobs using the seach terms " +
            "specified by the user in the selected job search profile.\r\n\r\n" +
            "The Job Scraper will automatically page through all of the job listings and in general " +
            "does not require any interaction from the user. The job scraper will automatically close the Google Chrome browser upon completion of searching " +
            "for jobs. Note that it may take a couple minutes before the results appear in the database if a large quantity of jobs were scraped. " +
            "Just keep using the Options \"Go To Default View\" button to refresh until the new jobs appear.\r\n\r\n" +
            "If you wish to change the amount of jobs that the scraper will try to retrieve, you can update the \"MAX_JOB_LISTING_INDEX\" property in the " +
            "appsettings.json file found in the AutoJobSearchJobScraper directory.\r\n\r\n" +
            "Sometimes Google will determine that an automated program is using their platform and will throw captchas (tests to ensure the user is a human, " +
            "usually by having them select the appropriate images) at the Selenium Chrome browser. " +
            "The application will detect if a captcha was displayed and will pause the job scraper until the user solves the captcha.\r\n\r\n" +
            "If you encounter a captcha, " +
            "simply solve the captcha so the Selenium Chrome browser does display the Google jobs search page. Then type \"CONTINUE\" into the console and the " +
            "web scraper will resume without any need for further user interaction.\r\n\r\n" +
            "In my testing, Google blocks any sort of request to their job search " +
            "page if it isn't being made by a browser. Therefore you cannot use a headless browser or pure HTTP requests to scrape jobs.";

        public static string AboutJobSearch => "The Job Search tab allows the user to specify what jobs they wish to search for, as well as any keywords and " +
            "sentiments they wish to define for scoring the scraped jobs.\r\n\r\n" +
            "To specify multiple search terms/keywords/sentiments, place each item on a " +
            "new line and end each line with a comma (ex. the Search Term area should look like...\r\n\r\n" +
            "Job To Search For #1,\r\n" +
            "Job To Search For #2\r\n\r\n" +
            "Therefore do not include commas anywhere " +
            "except to define the end of a search term/keyword/sentiment.\r\n\r\n" +
            "The user can start the job scraper for the selected profile by pressing the \"Execute Job Search\" button.\r\n\r\n" +
            "Multiple search profiles can be created by the user if you wish to split up your job searches and scoring keywords/sentiments.";

        public static string AboutThisProject => "This project was built in October 2023 as a tool to help automate my job search. " +
                                            "This user interface was built with AvaloniaUI. The web scraping is accomplished using Selenium. Both the GUI and the " +
            "web scraper read/save data to a local shared SQLite database. All code is written in C# and XAML.\r\n\r\n" +
            "Project Website: https://chrisbrown-01.github.io/AutoJobSearch/\r\n\r\n" +
            "GitHub: https://github.com/chrisbrown-01/AutoJobSearch\r\n\r\n";
    }
}