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
        public static async Task CreateDb()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();

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

                await connection.ExecuteAsync(sql);

                sql = @"
                     CREATE TABLE IF NOT EXISTS ApplicationLinks(
                     Id INTEGER PRIMARY KEY AUTOINCREMENT,
                     JobListingId INTEGER,
                     Link TEXT,
                     Link_RawHTML TEXT,
                     FOREIGN KEY(JobListingId) REFERENCES JobListings(Id)
                     );";

                await connection.ExecuteAsync(sql);
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

        public static async Task GetAllLinks()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sql = "SELECT * FROM ApplicationLinks;";
                //var sql = "SELECT * FROM ApplicationLinks WHERE JobListingId = your_specific_id;";

                var applicationLinks = await connection.QueryAsync<ApplicationLink>(sql);

                Console.WriteLine();
            }
        }
    }
}
