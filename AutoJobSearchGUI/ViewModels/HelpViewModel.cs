namespace AutoJobSearchGUI.ViewModels
{
    public partial class HelpViewModel : ViewModelBase // Needs to be public for View previewer to work
    {
        public static string AboutJobBoard =>
            "The Job Board tab displays the scraped job listings in a table format. Select a job and double click on it, " +
            "or press the \"Open Selected Job\" button to view the full job description and weblinks to apply at.\r\n\r\n" +
            "Click the check boxes to update the current status of the job listing.\r\n\r\n" +
            "After executing the job scraper, you will need to use the Advanced Query \"Clear All Filters\" button or the Options " +
            "\"Go To Default View\" button to see the new jobs. Note that it may take a couple of minutes before the new jobs are " +
            "available for display.\r\n\r\n" +
            "The user can resize the width of columns and sort the displayed job listings by clicking the column headers. Use the " +
            "\"Previous/Next Page\" buttons to navigate through the job listings.\r\n\r\n" +
            "Query the database for specific results using the \"Advanced Query\" menu and enabling the appropriate filters.\r\n\r\n" +
            "The user can archive (hide) jobs by right clicking on a job listing and selecting \"Archive/Hide Job\". By default, " +
            "the GUI will not display any jobs that were hidden unless they use the Advanced Query option.";

        public static string AboutJobListing =>
            "After opening a job listing via the Job Board, the job will be displayed in the Job Listing view.\r\n\r\n" +
            "Click on the status buttons to update the current status of the job listing.\r\n\r\n" +
            "The job listing opens in a read-only mode by default. You can edit most fields by selecting \"Options --> Edit Job\". " +
            "Modifying the status buttons does not require using the Edit Job option. \r\n\r\n" +
            "You can also use the Options button to upload relevant files and link them to each job listing, as well as create associated " +
            "contacts for the job.\r\n\r\n" +
            "Use the \"Previous/Next Job\" buttons to navigate through the job listings.\r\n\r\n";

        public static string AboutJobScoring =>
            "Job scoring is accomplished using the keywords and sentiments specified by the user in the job search profile section. " +
            "The keywords and sentiments are not case sensitive.\r\n\r\n" +
            "The score applied to the job increases by 1 every time the job description contains a positive keyword, and decreases by 1 " +
            "every time the description contains a negative keyword.\r\n\r\n" +
            "The score increases by 1 every time the job description exceeds a similarity threshold based on how similar it is to each " +
            "of the positive sentiments, and likewise for negative sentiments.\r\n\r\n" +
            "Sentiments should be at least 1 sentence long but should not be longer than a few sentences long.\r\n\r\n" +
            "Sentiment scoring is accomplished using fuzzy string matching with the FuzzySharp library.\r\n\r\n" +
            "If you wish to change the scoring thresholds used by the FuzzySharp library, you can update the \"FUZZ_RATIO_THRESHOLD\" " +
            "properties in the appsettings.json file found in the AutoJobSearchJobScraper directory.";

        public static string AboutJobScraper =>
            "The Job Scraper uses Selenium to open web browsers and navigate to Indeed and Google Jobs. Selenium then searches for jobs using " +
            "the search terms specified by the user in the Job Search profile.\r\n\r\n" +
            "The Job Scraper will automatically page through all of the job listings and does not require any interaction from the user. " +
            "The job scraper will automatically close the web browsers upon completion of searching for jobs. Note that it may take a couple " +
            "minutes before the results appear in the database if a large quantity of jobs were scraped. Keep using the Options \"Go To Default " +
            "View\" button to refresh until the new jobs appear.\r\n\r\n" +
            "If you wish to change the amount of jobs that the scraper will try to retrieve, you can update the \"Max Search Index\" property " +
            "of the Job Search profile.\r\n\r\n" +
            "Sometimes Indeed and Google will determine that an automated program is using their platform and will throw captchas (tests to " +
            "ensure the user is a human) at the web browsers. Multiple web browsers are opened to reduce the frequency of these captchas from " +
            "occurring. It is also recommended to use a VPN while using AutoJobSearch. However, if they do occur, AutoJobSearch will detect it " +
            "and automatically restart the web browser, then navigate back to the most recent web page. This is usually able to bypass the " +
            "captcha without any user input. If this is still unsuccessful, you can also modify the appsettings.json file found in the " +
            "AutoJobSearchJobScraper directory to set BROWSER_DEBUG_MODE to \"True\", then follow the instructions in the command terminal to " +
            "step through the website navigation one page at a time.\r\n\r\n" +
            "In my testing, Google blocks any sort of request to their job search page if it isn't being made by a browser. Therefore you " +
            "cannot use a headless browser or pure HTTP requests to scrape jobs.";

        public static string AboutJobSearch =>
            "The Job Search tab allows the user to specify what jobs they wish to search for, as well as any keywords and sentiments they wish " +
            "to define for scoring the scraped jobs.\r\n\r\n" +
            "Ensure that you are specifying the city and country in every job search term. For example, \"Programming jobs Los Angeles USA\".\r\n\r\n" +
            "To specify multiple search terms/keywords/sentiments, place each item on a new line and end each line with a comma. For example, " +
            "the Search Term area should look like:\r\n\r\n" +
            "Job To Search For #1,\r\n" +
            "Job To Search For #2\r\n\r\n" +
            "Therefore do not include commas anywhere except to define the end of a search term/keyword/sentiment.\r\n\r\n" +
            "The user can start the job scraper for the selected profile by pressing the \"Execute Job Search\" button.\r\n\r\n" +
            "Multiple search profiles can be created by the user if you wish to split up your job searches and scoring keywords/sentiments.\r\n\r\n" +
            "You can change the amount of jobs to be scraped by modifying the value of the \"Max Search Index\". A lower value corresponds to " +
            "less job listings retrieved by the application. The default value is 150, though this does not always mean that exactly 150 job " +
            "listings will be found for each job search term.";

        public static string AboutThisProject =>
            "This project was started in September 2023 as a tool to help automate my job search. This user interface was built with " +
            "AvaloniaUI. The web scraping is accomplished using Selenium. Both the GUI and the web scraper read/save data to a local " +
            "shared SQLite database. All code is written in C# and XAML.\r\n\r\n" +
            "Project Website: https://chrisbrown-01.github.io/AutoJobSearch/\r\n\r\n" +
            "GitHub: https://github.com/chrisbrown-01/AutoJobSearch\r\n\r\n";
    }
}