﻿using AutoJobSearchConsoleApp.Models;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite; // TODO: uninstall packages where they're not required
using System.Diagnostics;
using System.Text;
using static OpenQA.Selenium.PrintOptions;

namespace AutoJobSearchShared
{
    public class SQLiteDb
    {
        public static async Task<IEnumerable<Models.JobListing>> ExecuteJobBoardQuery(Models.JobListing jobListingQuery)
        {
            Debug.WriteLine($"Getting job listings per user job board query"); // TODO: proper logging

            var jobListings = new List<Models.JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING))
            {
                await connection.OpenAsync();

                var sqlQuery = @"SELECT * FROM JobListings
                        WHERE IsAppliedTo = @IsAppliedTo 
                        AND IsInterviewing = @IsInterviewing
                        AND IsRejected = @IsRejected
                        AND IsFavourite = @IsFavourite
                        AND IsHidden = @IsHidden;";

                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(
                    sqlQuery, 
                    new 
                    { 
                        IsAppliedTo = jobListingQuery.IsAppliedTo,
                        IsInterviewing = jobListingQuery.IsInterviewing,
                        IsRejected = jobListingQuery.IsRejected,
                        IsFavourite = jobListingQuery.IsFavourite,
                        IsHidden = jobListingQuery.IsHidden
                    });

                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }

        public static async Task UpdateNotesById(int id, string notes)
        {
            Debug.WriteLine($"Updating notes for id {id}"); // TODO: proper logging

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) // TODO: db connection pooling
            {
                await connection.OpenAsync();

                var sqlQuery = "UPDATE JobListings SET Notes = @Notes WHERE Id = @Id;";
                await connection.ExecuteAsync(sqlQuery, new { Notes = notes, Id = id });
            }
        }

        public static async Task<IEnumerable<Models.JobListing>> GetJobListings(int page = 0, int pageSize = 25)
        {
            Debug.WriteLine($"Getting job listings for page {page} and pagesize {pageSize}"); // TODO: proper logging

            var jobListings = new List<Models.JobListing>();

            using (var connection = new SqliteConnection(Constants.SQLITE_CONNECTION_STRING)) 
            {
                await connection.OpenAsync();

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
                         FROM JobListings
                         LIMIT @PageSize OFFSET @PageOffset";

                var jobListingsQuery = await connection.QueryAsync<Models.JobListing>(sqlQuery, new { PageSize = pageSize, PageOffset = page * pageSize });
                jobListings = jobListingsQuery.ToList(); // TODO: improve, perform null checking?
            }

            return jobListings;
        }


        public static async Task<string> GetNotesById(int id)
        {
            Debug.WriteLine($"Getting notes for listing id {id}"); // TODO: proper logging

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
            Debug.WriteLine($"Getting links for listing id {id}"); // TODO: proper logging

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