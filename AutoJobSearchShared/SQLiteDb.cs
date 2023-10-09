using AutoJobSearchConsoleApp.Models;
using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite; // TODO: uninstall packages where they're not required
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text;

namespace AutoJobSearchShared
{
    public class SQLiteDb
    {
        // TODO: implement logger, remove statics?

        //private readonly ILogger _logger;
        //public SQLiteDb(ILogger logger)
        //{
        //    _logger = logger;
        //}

        public static async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfiles()
        {
            Debug.WriteLine($"Getting all job search profile"); // TODO: proper logging

            IEnumerable<JobSearchProfile> profiles;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT * FROM JobSearchProfiles;";

                profiles = await connection.QueryAsync<JobSearchProfile>(sqlQuery);
            }

            return profiles;
        }

        public static async Task CreateNewJobSearchProfile(JobSearchProfile profile)
        {
            Debug.WriteLine($"Creating new job search profile"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                    const string sql = @"
                INSERT INTO JobSearchProfiles 
                (ProfileName, Searches, KeywordsPositive, KeywordsNegative, SentimentsPositive, SentimentsNegative) 
                VALUES (@ProfileName, @Searches, @KeywordsPositive, @KeywordsNegative, @SentimentsPositive, @SentimentsNegative)";

                await connection.ExecuteAsync(sql, profile);
            }
        }

        public static async Task<IQueryable<Models.JobListing>> ExecuteJobBoardAdvancedQuery(
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
                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery);
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
                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery);
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

                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery);
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task<Models.JobListing> GetJobListingDetails(int id)
        {
            Debug.WriteLine($"Getting details for listing id {id}"); // TODO: proper logging

            var jobListing = new Models.JobListing();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var jobListingTableQuery = "SELECT Description, Notes From JobListings Where Id = @Id;";
                var jobListingTableResult = await connection.QuerySingleAsync<Models.JobListing>(jobListingTableQuery, new { Id = id });

                var applicationLinksTableQuery = "SELECT Link FROM ApplicationLinks Where JobListingId = @Id;";
                var applicationLinksTableResult = await connection.QueryAsync<string>(applicationLinksTableQuery, new { Id = id });

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

        public static async Task UpdateJobListingBoolProperty(DbBoolField columnName, bool value, int id) // TODO: convert to use enum
        {
            Debug.WriteLine($"Updating boolean for id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();
                string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id"; // TODO: change to stored procedure
                await connection.ExecuteAsync(sql, new { Value = value, Id = id });
            }
        }

        public static async Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id) // TODO: convert to use enum
        {
            Debug.WriteLine($"Updating notes for id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) // TODO: db connection pooling
            {
                await connection.OpenAsync();

                var sqlQuery = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id;";
                await connection.ExecuteAsync(sqlQuery, new { Value = value, Id = id });
            }
        }

    }
}