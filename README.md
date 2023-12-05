  ![AutoJobSearch](/Images/Logo.png)

Use AutoJobSearch to help automate your job search! AutoJobSearch is a free desktop application that automatically finds jobs according to your search terms and provides a platform to better manage your job searching experience. Key features of this tool include:

- Only find jobs that you haven't seen before and minimize duplicated job listings
- Score jobs based on user-defined keywords and sentiments
- Keep track of which jobs that are applied to/interviewing/rejected
- Narrow down displayed job listings with the sort, search and filter options
- Save multiple search profiles so you can apply different keyword/sentiment scorings to different search terms
- Cross-platform: download for Windows, Linux or MacOS

![Screen recording gif](/Images/AutoJobSearchDemo.gif)

## How To Run

Download the zip file for your operating system (Windows, Linux or Mac) from the [Releases](https://github.com/chrisbrown-01/AutoJobSearch/releases) page. Unzip the folder. 

*Windows*:
- Run the `AutoJobSearchGUI.exe` executable found in the AutoJobSearchGUI folder
- If Windows Defender blocks running of the executable, try to select the `More info` → `Run Anyway` option


*Linux*:
- In the directory where you extracted the folder, modify privileges by running the following terminal command or equivalent: `chmod 755 -R AutoJobSearch.Linux-x64/`
- Run program by running the following terminal command: `./AutoJobSearch.Linux-x64/AutoJobSearchGUI/AutoJobSearchGUI`


*Mac*:
- To be updated. I have not been able to personally test this on a Mac yet.
- Please submit a pull request with instructions if you are a Mac user that wishes to contribute.


## How To Use

1. Navigate to the **Job Search** tab.
2. Start typing in the jobs you want to search for in the *Search Terms* box. To search for multiple jobs/search terms, seperate each of them with a comma (example: job to search #1, job to search #2).
3. If you wish to apply scoring to the job listings, enter your keywords and sentiments into the appropriate boxes. Keywords and sentiments are not case sensitive. Sentiments should only be about 1 sentence. To search for multiple keywords/sentiments, seperate each of them with a comma.
4. Click *Execute Job Search*. This will open a Chrome browser and begin the automated job search - no user interaction is required.
5. Sometimes Google will detect that an automated program is using their services and will throw captchas at the Chrome browser to make it prove a human is controlling it. AutoJobSearch will automatically detect if this has happened and will pause the job search until the user solves the captcha. Simply solve the captcha in the Chrome browser, and once the page is back to rendering the Google Job Search page with listings, type `CONTINUE` into the terminal console and press enter to resume the automated search.
6. After the job search has concluded, the Chrome browser will close automatically. Navigate back to the **Job Board** tab.
7. Newly downloaded jobs will not be shown until you refresh the page. Click *Options* → *Go To Default View* to refresh the page. This should now show all job listings, starting with the most recently downloaded.
8. To view a job description, click on a job listing then press the *Open Selected Job* button.
9. To apply filters and sorting, apply the appropriate *Advanced Query* options then press the *Execute Query* button.

---
***Job Board***

![Job Board](/Images/JobBoard.png)

---
***Job Listing & Description***

![Job Listing](/Images/JobListing.png)

---
***Job Search***

![Job Search](/Images/JobSearch.png)

---
***Job Board Query Options***

![Job Board Query Options](/Images/AdvancedQuery.png)

## How It Works

AutoJobSearch is built with .NET 7 and AvaloniaUI. The tool uses a Selenium Chrome browser to search for jobs defined by the user then downloads all listings indexed by the Google Job Search tool. After filtering out any duplicate jobs or listings that have been previously downloaded, each job description is parsed for keywords and scored for each positive/negative keyword that is found. Fuzzy string matching is also used to apply scoring for sentiments using the FuzzySharp library. The scored jobs then get saved to a local SQLite database on the user's computer. The GUI displays all job listings saved in the database and provides options to filter and sort the displayed listings. All database interactions are performed using the Dapper micro ORM. 

## Misc. Comments

- The GUI and Job Scraper have seperate executables so you can further automate your job search by running the job scraper without needing to go through the GUI
- Modify the `appsettings.json` file in the *AutoJobSearchJobScraper* folder to change the maximum amount of jobs the tool will attempt to scrape for each search term. You can also change the keywords that the job scraper will use to extract the job description from the Google Job Search page, as well as the threshold values used for the fuzzy string matching sentiment scorings.
- There are methods in the GUI project that use an `async void` return type. I am aware that this is normally a huge anti-pattern in .NET, but for Avalonia this is the only way I can properly use `async await` inside the method while still being able to propogate thrown exceptions up to the logger. Using `async Task` return types with Avalonia view models actually prevent exceptions from being logged during my experiments. 
