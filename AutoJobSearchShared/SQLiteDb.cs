using AutoJobSearchConsoleApp.Models;
using Dapper;
using Microsoft.Data.Sqlite; // TODO: uninstall packages where they're not required

namespace AutoJobSearchShared
{
    public class SQLiteDb
    {
        public static async Task<IEnumerable<JobListing>> GetAllJobListings()
        {
            var jobListings = new List<JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                // var sqlQuery = "SELECT * FROM JobListings";
                var sqlQuery = @"SELECT 
                                 Id, 
                                 SearchTerm, 
                                 CreatedAt, 
                                 Description, 
                                 Score, 
                                 IsAppliedTo,
                                 IsInterviewing,
                                 IsRejected
                                 FROM JobListings";

                var jobListingsQuery = await connection.QueryAsync<JobListing>(sqlQuery);
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

    }
}