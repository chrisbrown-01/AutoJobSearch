  ![AutoJobSearch](/Images/Logo.png)

Use AutoJobSearch to help automate your job search! AutoJobSearch is a free desktop application that automatically finds jobs according to your search terms and provides an optimal platform for managing your job search experience. Key features of this tool includes:

- Only find jobs that you haven't seen before and minimize duplicated job listings
- Score jobs based on user-defined keywords and sentiments
- Keep track of which jobs that are applied to/interviewing/rejected
- Narrow your job search with advanced sorting, searching and filtering options
- Save multiple search profiles so you can apply different keyword/sentiment scorings to different search terms
- Cross-platform: download for Windows, Linux or MacOS

![Screen recording gif](/Images/AutoJobSearchDemo.gif)

## How To Run

Download the zip file for your operating system (Windows, Linux or Mac) from the Releases section. Unzip the folder. 

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


## How It Works

AutoJobSearch is a tool built with .NET 7 and AvaloniaUI. The tool uses a Selenium Chrome browser to search for jobs defined by the user then downloads all listings indexed by the Google Job Search tool. After filtering out any duplicate jobs or listings that have been previously downloaded, each job description is parsed for keywords and scored for each positive/negative keyword that is found. Fuzzy string matching is also used to apply scoring for sentiments using the FuzzySharp library. The scored jobs then get saved to a local SQLite database on the user's computer. The GUI displays all job listings saved in the database and provides options to filter and sort the displayed listings. 
