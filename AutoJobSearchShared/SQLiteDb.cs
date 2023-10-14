using AutoJobSearchConsoleApp.Models;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite; // TODO: uninstall packages where they're not required
using System.Diagnostics;
using System.Text;

namespace AutoJobSearchShared
{
    public class SQLiteDb // TODO: extract interface
        // TODO: connection pooling/sharing
    {
        // TODO: implement logger, remove statics?

        //private readonly ILogger _logger;
        //public SQLiteDb(ILogger logger)
        //{
        //    _logger = logger;
        //}

        // TODO: delete all records methods

        public static async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            JobSearchProfile? profile;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                const string sql = "SELECT * FROM JobSearchProfiles WHERE Id = @Id;";

                profile = await connection.QuerySingleOrDefaultAsync<JobSearchProfile>(sql, new { Id = id });
            }

            return profile;
        }

        public static async Task<IEnumerable<string>> GetAllApplicationLinks()
        {
            IEnumerable<string> applicationLinks;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                const string sql = "SELECT Link FROM ApplicationLinks;";

                applicationLinks = await connection.QueryAsync<string>(sql); // TODO: testing if no records exist
            }

            return applicationLinks;
        }

        public static async Task SaveJobListings(IEnumerable<Models.JobListing> jobListings)
        {
            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                const string insertJobListingSQL = @"
                INSERT INTO JobListings (
                SearchTerm, 
                CreatedAt, 
                Description_Raw, 
                Description, 
                Score, 
                IsAppliedTo, 
                IsInterviewing, 
                IsRejected, 
                IsFavourite,
                IsHidden,
                Notes
                ) VALUES (
                @SearchTerm, 
                @CreatedAt, 
                @Description_Raw, 
                @Description, 
                @Score, 
                @IsAppliedTo, 
                @IsInterviewing, 
                @IsRejected, 
                @IsFavourite, 
                @IsHidden,
                @Notes
                );";

                const string getLastInsertIdSQL = "SELECT last_insert_rowid();";

                const string insertApplicationLinksSQL = "INSERT INTO ApplicationLinks (JobListingId, Link, Link_RawHTML) Values (@JobListingId, @Link, @Link_RawHTML)"; // TODO: make const/static, try to complete in batches

                foreach (var job in jobListings)
                {
                    await connection.ExecuteAsync(insertJobListingSQL, job);
                    var jobListingId = await connection.QuerySingleAsync<int>(getLastInsertIdSQL);

                    foreach (var link in job.ApplicationLinks)
                    {
                        link.JobListingId = jobListingId;

                        await connection.ExecuteAsync(insertApplicationLinksSQL, link);
                    }
                }
            }
        }

        public static async Task DeleteJobSearchProfile(int id)
        {
            Debug.WriteLine($"Getting job search profile id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"DELETE FROM JobSearchProfiles WHERE Id = @Id;";

                await connection.ExecuteAsync(sqlQuery, new {Id = id});
            }
        }

        public static async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            Debug.WriteLine($"Getting all job search profile"); // TODO: proper logging

            IEnumerable<JobSearchProfile> profiles;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                const string sql = "SELECT * FROM JobSearchProfiles";

                profiles = await connection.QueryAsync<JobSearchProfile>(sql); // TODO: testing if no records in table
            }

            return profiles;
        }

        public static async Task<JobSearchProfile> CreateJobSearchProfile(JobSearchProfile profile)
        {
            JobSearchProfile? newProfile;

            Debug.WriteLine($"Creating new job search profile"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();
                // TODO: change all sql strings to const
                const string sql = @"
            INSERT INTO JobSearchProfiles 
            (ProfileName, Searches, KeywordsPositive, KeywordsNegative, SentimentsPositive, SentimentsNegative) 
            VALUES (@ProfileName, @Searches, @KeywordsPositive, @KeywordsNegative, @SentimentsPositive, @SentimentsNegative);
            SELECT * FROM JobSearchProfiles WHERE Id = last_insert_rowid();";

                newProfile = await connection.QuerySingleAsync<JobSearchProfile>(sql, profile);
            }

            return newProfile;
        }

        public static async Task<IQueryable<Models.JobListing>> ExecuteJobBoardAdvancedQuery( // TODO: change name to JobListingQuery?
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            Debug.WriteLine($"Getting job listings per user job board query"); // TODO: proper logging

            IEnumerable<Models.JobListing> jobListings;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT Id, 
                 SearchTerm, 
                 CreatedAt, 
                 Description, 
                 Score, 
                 IsAppliedTo,
                 IsInterviewing,
                 IsRejected,
                 IsFavourite,
                 IsHidden,
                 Notes FROM JobListings
                 WHERE IsAppliedTo = @IsAppliedTo 
                 AND IsInterviewing = @IsInterviewing
                 AND IsRejected = @IsRejected
                 AND IsFavourite = @IsFavourite
                 AND IsHidden = False;";

                // TODO: testing if no records exist
                jobListings = await connection.QueryAsync<Models.JobListing>( 
                    sqlQuery,
                    new
                    {
                        IsAppliedTo = isAppliedTo,
                        IsInterviewing = isInterviewing,
                        IsRejected = isRejected,
                        IsFavourite = isFavourite
                    });
            }

            return jobListings.AsQueryable();
        }

        public static async Task<IEnumerable<Models.JobListing>> GetHiddenJobListings()
        {
            Debug.WriteLine($"Getting hidden job listings"); // TODO: proper logging

            var jobListings = new List<Models.JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT 
                         Id, 
                         SearchTerm, 
                         CreatedAt, 
                         SUBSTR(Description, 1, 200) AS Description,
                         Score, 
                         IsAppliedTo,
                         IsInterviewing,
                         IsRejected,
                         IsFavourite,
                         IsHidden
                         FROM JobListings
                         WHERE IsHidden = True
                        ORDER BY Id DESC";
                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery); // TODO: testing if no records in db
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task<IEnumerable<Models.JobListing>> GetFavouriteJobListings()
        {
            Debug.WriteLine($"Getting favourite job listings"); // TODO: proper logging

            var jobListings = new List<Models.JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT 
                         Id, 
                         SearchTerm, 
                         CreatedAt, 
                         SUBSTR(Description, 1, 200) AS Description,
                         Score, 
                         IsAppliedTo,
                         IsInterviewing,
                         IsRejected,
                         IsFavourite,
                         IsHidden
                         FROM JobListings
                         WHERE IsFavourite = True
                        ORDER BY Id DESC";
                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery); // TODO: testing for no records in db
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task<IEnumerable<Models.JobListing>> GetAllJobListings()
        {
            Debug.WriteLine($"Getting all job listings"); // TODO: proper logging

            var jobListings = new List<Models.JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT 
                     Id, 
                     SearchTerm, 
                     CreatedAt, 
                     SUBSTR(Description, 1, 200) AS Description,
                     Score, 
                     IsAppliedTo,
                     IsInterviewing,
                     IsRejected,
                     IsFavourite,
                     IsHidden
                 FROM JobListings
                 WHERE IsHidden = False
                 ORDER BY Id DESC";

                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery); // TODO: testing for no records in db
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task<Models.JobListing> GetJobListingDetails(int id) // TODO: rename with id and async
        {
            Debug.WriteLine($"Getting details for listing id {id}"); // TODO: proper logging

            var jobListing = new Models.JobListing();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var jobListingTableQuery = "SELECT Description, Notes From JobListings Where Id = @Id;";
                var jobListingTableResult = await connection.QuerySingleAsync<Models.JobListing>(jobListingTableQuery, new { Id = id });

                var applicationLinksTableQuery = "SELECT Link FROM ApplicationLinks Where JobListingId = @Id;";
                var applicationLinksTableResult = await connection.QueryAsync<string>(applicationLinksTableQuery, new { Id = id }); // TODO: testing if no records exist

                jobListing.Description = jobListingTableResult.Description;
                jobListing.Notes = jobListingTableResult.Notes;

                if (applicationLinksTableResult != null)
                {
                    StringBuilder sb = new();

                    foreach (var link in applicationLinksTableResult)
                    {
                        sb.AppendLine(link);
                    }

                    jobListing.ApplicationLinksString = sb.ToString();
                }
            }

            return jobListing;
        }

        public static async Task UpdateJobListingBoolProperty(JobListingsBoolField columnName, bool value, int id) 
        {
            Debug.WriteLine($"Updating boolean for id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();
                string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id"; 
                await connection.ExecuteAsync(sql, new { Value = value, Id = id });
            }
        }

        public static async Task UpdateJobSearchProfileStringProperty(JobSearchProfilesStringField columnName, string value, int id)
        {
            Debug.WriteLine($"Updating string for job profile id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();
                string sql = $"UPDATE JobSearchProfiles SET {columnName} = @Value WHERE Id = @Id"; 
                await connection.ExecuteAsync(sql, new { Value = value, Id = id });
            }
        }

        public static async Task UpdateJobListingStringProperty(JobListingsStringField columnName, string value, int id) 
        {
            Debug.WriteLine($"Updating notes for id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();

                var sqlQuery = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id;";
                await connection.ExecuteAsync(sqlQuery, new { Value = value, Id = id });
            }
        }

    }
}