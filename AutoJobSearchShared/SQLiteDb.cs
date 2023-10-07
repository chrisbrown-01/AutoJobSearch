﻿using AutoJobSearchConsoleApp.Models;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite; // TODO: uninstall packages where they're not required
using System.Text;

namespace AutoJobSearchShared
{
    public class SQLiteDb
    {
        public static async Task<IEnumerable<Models.JobListing>> GetAllJobListings()
        {
            var jobListings = new List<Models.JobListing>();

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
                                 IsRejected,
                                 IsFavourite
                                 FROM JobListings";

                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery);
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task<string> GetNotesById(int id)
        {
            string notes = string.Empty;

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var notesQuery = "SELECT Notes From JobListings Where Id = @Id;";
                var query = await connection.QuerySingleAsync<string>(notesQuery, new { Id = id });

                if (query != null) notes = query;
            }

            return notes;
        }

        public static async Task<string> GetApplicationLinksById(int id)
        {
            string links = string.Empty;  

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var linksQuery = "SELECT Link FROM ApplicationLinks Where JobListingId = @Id;";
                var query = await connection.QueryAsync<string>(linksQuery, new { Id = id });

                if(query != null)
                {
                    StringBuilder sb = new();

                    foreach(var link in query)
                    {
                        sb.AppendLine(link);
                    }

                    links = sb.ToString();
                }
            }

            return links;
        }

        public static async Task UpdateDatabaseBoolPropertyById(string columnName, bool value, int id) // TODO: better naming
        {
            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();
                string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id"; // TODO: change to stored procedure
                await connection.ExecuteAsync(sql, new { Value = value, Id = id });
            }
        }

    }
}