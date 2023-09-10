using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchConsoleApp
{
    internal class SQLiteTesting
    {
        private const string CONNECTION_STRING = "Data Source=..\\..\\..\\AutoJobSearch.db";

        public static void CreateDb()
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                connection.Open();

                var sql = @"
            CREATE TABLE IF NOT EXISTS JobListings (
                Id INTEGER PRIMARY KEY,
                SearchTerm TEXT,
                CreatedAt TEXT,
                Description_Raw TEXT,
                Description TEXT,
                ApplicationLinks_Raw TEXT,
                ApplicationLinks TEXT,
                Score INTEGER,
                IsAppliedTo INTEGER,
                IsInterviewing INTEGER,
                IsRejected INTEGER,
                Notes TEXT
            );";

                connection.Execute(sql);
            }
        }

        public static async Task PopulateDb(List<JobListing> jobListings)
        {
            using (var connection = new SqliteConnection(CONNECTION_STRING))
            {
                await connection.OpenAsync();

                //var job = new
                //{
                //    SearchTerm = "Software Engineer",
                //    CreatedAt = DateTime.Now.ToString(),
                //    Description_Raw = "This is a raw description",
                //    Description = "This is a description",
                //    ApplicationLinks_Raw = "www.example.com",
                //    ApplicationLinks = "www.example.com",
                //    Score = 100,
                //    IsAppliedTo = 0,
                //    IsInterviewing = 0,
                //    IsRejected = 0,
                //    Notes = "These are some notes"
                //};

                var sql = @"
            INSERT INTO JobListings (
                SearchTerm, 
                CreatedAt, 
                Description_Raw, 
                Description, 
                ApplicationLinks_Raw, 
                ApplicationLinks, 
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
                @ApplicationLinks_Raw, 
                @ApplicationLinks, 
                @Score, 
                @IsAppliedTo, 
                @IsInterviewing, 
                @IsRejected, 
                @Notes
            );";

                foreach(var job in jobListings)
                {
                    // TODO: serialization of List<string> into single string variable done here.
                    await connection.ExecuteAsync(sql, job);
                }           
            }
        }
    }
}
