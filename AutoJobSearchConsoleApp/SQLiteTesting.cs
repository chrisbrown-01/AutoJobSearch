using AutoJobSearchConsoleApp.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutoJobSearchConsoleApp
{
    internal class SQLiteTesting
    {
        private const string CONNECTION_STRING = "Data Source=..\\..\\..\\AutoJobSearch.db";

        // TODO: improve readability, run checks to ensure database does/does not exist for automated install
        public static void CreateDb()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var sql = @"
                CREATE TABLE IF NOT EXISTS JobListings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SearchTerm TEXT,
                CreatedAt TEXT,
                Description_Raw TEXT,
                Description TEXT,
                Score INTEGER,
                IsAppliedTo INTEGER,
                IsInterviewing INTEGER,
                IsRejected INTEGER,
                Notes TEXT
                );";

                connection.Execute(sql);

                sql = @"
                     CREATE TABLE IF NOT EXISTS ApplicationLinks(
                     Id INTEGER PRIMARY KEY AUTOINCREMENT,
                     JobListingId INTEGER,
                     Link TEXT,
                     Link_RawHTML TEXT,
                     FOREIGN KEY(JobListingId) REFERENCES JobListings(Id)
                     );";

                connection.Execute(sql);
            }
        }

        public static async Task PopulateDb(List<JobListing> jobListings)
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var insertJobListingSQL = @"
                INSERT INTO JobListings (
                SearchTerm, 
                CreatedAt, 
                Description_Raw, 
                Description, 
                Score, 
                IsAppliedTo, 
                IsInterviewing, 
                IsRejected, 
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
                @Notes
                );";

                var getLastInsertIdSQL = "SELECT last_insert_rowid();";

                var insertApplicationLinksSQL = "INSERT INTO ApplicationLinks (JobListingId, Link, Link_RawHTML) Values (@JobListingId, @Link, @Link_RawHTML)";

                foreach (var job in jobListings)
                {
                    await connection.ExecuteAsync(insertJobListingSQL, job);
                    var jobListingId = await connection.QuerySingleAsync<int>(getLastInsertIdSQL);

                    foreach (var link in job.ApplicationLinks)
                    {
                        await connection.ExecuteAsync(insertApplicationLinksSQL, new ApplicationLink() { 
                            JobListingId = jobListingId, 
                            Link = link.Link, 
                            Link_RawHTML = link.Link_RawHTML });
                    }
                }           
            }
        }
    }
}
