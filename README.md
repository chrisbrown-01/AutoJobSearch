  ![AutoJobSearch](/Images/Logo.png)

Use AutoJobSearch to help automate your job search! AutoJobSearch is a free desktop application that automatically finds jobs according to your search terms and provides a platform to better manage your job searching experience. Key features of this tool include:

- Only find jobs that you haven't seen before, eliminating duplicate job listings
- Improves the relevance of your search results by applying scores based on user-defined keywords and sentiments
- Keep track of which jobs that you have applied to, are interviewing for, were rejected from, etc.
- Use the sort, search and filter options to find saved job listings that meet your search criteria
- Save multiple search profiles so you can apply different keyword/sentiment scorings to different search terms
- Attach relevant files and contact information to individual job listings
- Cross-platform: download for Windows, Linux or MacOS

![Screen recording gif](/Images/AutoJobSearchDemo.gif)

## How To Run

- Download the zip file for your operating system (Windows, Linux or Mac) from the [Releases](https://github.com/chrisbrown-01/AutoJobSearch/releases) page. 
- Unzip the folder. 
- It is recommended to use AutoJobSearch with a VPN or some sort of proxy so you do not get blocked by Google or Indeed.

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
2. Start typing in the jobs you want to search for in the *Search Terms* box. Ensure that you are specifying the city and country in every search term. To search for multiple jobs/search terms, seperate each of them with a comma.
3. If you wish to apply scoring to the job listings, enter your keywords and sentiments into the appropriate boxes. Keywords and sentiments are not case sensitive. Sentiments should only be about 1 sentence. To search for multiple keywords/sentiments, seperate each of them with a comma.
4. Click *Execute Job Search*. This will open multiple web browser and begin the automated job search. No user interaction is required - do not close the web browsers or console terminal, as they will close automatically once the search has completed.
5. Navigate back to the **Job Board** tab.
7. Newly downloaded jobs will not be shown until you refresh the page. Click *Options* → *Go To Default View* to refresh the page. This should now show all job listings, starting with the most recently downloaded.
8. To view a job listing, double-click on it, or select it then press the *Open Selected Job* button.
9. To apply filters, apply the appropriate *Advanced Query* options then press the *Execute Query* button.

## How It Works

AutoJobSearch is built with .NET 7 and AvaloniaUI. The tool uses Selenium web browsers to search for jobs defined by the user, then downloads the listings it finds on Indeed and Google Jobs. After filtering out any duplicate jobs or listings that have been previously downloaded, each job description is parsed for keywords and scored for each positive/negative keyword that is found. Fuzzy string matching is also used to apply scoring for sentiments using the FuzzySharp library. The scored jobs then get saved to a local SQLite database on the user's computer. The GUI displays all job listings saved in the database and provides options to filter and sort the displayed listings. All database interactions are performed using the Dapper micro ORM. 

## Misc. Comments

- AutoJobSearch is developed and tested for Canadian and American users. This tool will likely not work well if you are located outside of these countries. Submit an issue or pull request if you want support added for your country.
- The GUI and Job Scraper have seperate executables so you can further automate your job search by running the job scraper without needing to go through the GUI.
- Modify the `appsettings.json` file in the *AutoJobSearchJobScraper* folder to change the keywords that the job scraper will use for extracting the job descriptions from the web page, as well as the threshold values used for the fuzzy string matching sentiment scorings.

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
