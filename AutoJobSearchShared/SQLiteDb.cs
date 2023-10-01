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

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) // TODO: db connection pooling
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

        public static async Task UpdateDatabaseBoolPropertyById(string columnName, bool value, int id) // TODO: better naming
        {
            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) // TODO: db connection pooling
            {
                await connection.OpenAsync();
                string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id"; // TODO: change to stored procedure
                await connection.ExecuteAsync(sql, new { Value = value, Id = id });
            }
        }

    }
}