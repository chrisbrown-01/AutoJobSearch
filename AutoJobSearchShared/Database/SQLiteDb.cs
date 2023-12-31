﻿using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Text;

namespace AutoJobSearchShared.Database
{
    public class SQLiteDb : IAutoJobSearchDb
    {
        private const string DATABASE_FILE_NAME = "../AutoJobSearch.db";
        private readonly SqliteConnection connection;

        public SQLiteDb()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATABASE_FILE_NAME);
            var connectionString = $"Data Source={dbPath}";
            connection = new SqliteConnection(connectionString);

            if (!File.Exists(dbPath))
            {
                connection.Open();
                CreateTables();
            }
            else
            {
                connection.Open();
            }
        }

        public void CreateTables()
        {
            const string createJobListingsTableSQL = "CREATE TABLE JobListings (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "SearchTerm TEXT," +
                "CreatedAt TEXT," +
                "Description_Raw TEXT," +
                "Description TEXT," +
                "Notes TEXT," +
                "Score INTEGER," +
                "IsAppliedTo INTEGER," +
                "IsInterviewing INTEGER," +
                "IsRejected INTEGER," +
                "IsFavourite INTEGER," +
                "IsHidden INTEGER)";

            connection.Execute(createJobListingsTableSQL);

            const string createApplicationLinksTableSQL = "CREATE TABLE ApplicationLinks (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "JobListingId INTEGER," +
                 "Link TEXT," +
                 "Link_RawHTML TEXT," +
                 "FOREIGN KEY(JobListingId) REFERENCES JobListings(Id))";

            connection.Execute(createApplicationLinksTableSQL);

            const string createJobSearchProfilesTableSQL = "CREATE TABLE JobSearchProfiles (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "ProfileName TEXT," +
                 "Searches TEXT," +
                 "KeywordsPositive TEXT," +
                 "KeywordsNegative TEXT," +
                 "SentimentsPositive TEXT," +
                 "SentimentsNegative TEXT)";

            connection.Execute(createJobSearchProfilesTableSQL);
        }

        public async Task DeleteAllJobListingsAsync()
        {
            const string sql = "DELETE FROM ApplicationLinks; DELETE FROM JobListings;";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            const string sql = "SELECT * FROM JobSearchProfiles WHERE Id = @Id;";
            return await connection.QuerySingleOrDefaultAsync<JobSearchProfile>(sql, new { Id = id }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetAllApplicationLinksAsync()
        {
            const string sql = "SELECT Link FROM ApplicationLinks;";
            return await connection.QueryAsync<string>(sql).ConfigureAwait(false);
        }

        public async Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings)
        {
            const string insertJobListingSQL =
                "INSERT INTO JobListings (" +
                "SearchTerm, " +
                "CreatedAt, " +
                "Description_Raw, " +
                "Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite," +
                "IsHidden, " +
                "Notes" +
                ") VALUES (" +
                "@SearchTerm, " +
                "@CreatedAt, " +
                "@Description_Raw, " +
                "@Description, " +
                "@Score, " +
                "@IsAppliedTo, " +
                "@IsInterviewing, " +
                "@IsRejected, " +
                "@IsFavourite, " +
                "@IsHidden, " +
                "@Notes);";

            const string getLastInsertIdSQL = "SELECT last_insert_rowid();";

            const string insertApplicationLinksSQL =
                "INSERT INTO ApplicationLinks (" +
                "JobListingId, " +
                "Link, " +
                "Link_RawHTML" +
                ") VALUES (" +
                "@JobListingId, " +
                "@Link, " +
                "@Link_RawHTML)";

            using var transaction = await connection.BeginTransactionAsync().ConfigureAwait(false);

            try
            {
                foreach (var job in jobListings)
                {
                    await connection.ExecuteAsync(insertJobListingSQL, job, transaction).ConfigureAwait(false);
                    var jobListingId = await connection.QuerySingleAsync<int>(getLastInsertIdSQL).ConfigureAwait(false);

                    foreach (var link in job.ApplicationLinks)
                    {
                        link.JobListingId = jobListingId;
                        await connection.ExecuteAsync(insertApplicationLinksSQL, link, transaction).ConfigureAwait(false);
                    }
                }

                await transaction.CommitAsync().ConfigureAwait(false);
            }
            catch
            {
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            const string sql = "DELETE FROM JobSearchProfiles WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            const string sql = "SELECT * FROM JobSearchProfiles";
            return await connection.QueryAsync<JobSearchProfile>(sql).ConfigureAwait(false);
        }

        public async Task<JobSearchProfile> CreateJobSearchProfileAsync(JobSearchProfile profile)
        {
            const string sql =
                "INSERT INTO JobSearchProfiles (" +
                "ProfileName, " +
                "Searches, " +
                "KeywordsPositive, " +
                "KeywordsNegative, " +
                "SentimentsPositive, " +
                "SentimentsNegative" +
                ") VALUES (" +
                "@ProfileName, " +
                "@Searches, " +
                "@KeywordsPositive, " +
                "@KeywordsNegative, " +
                "@SentimentsPositive, " +
                "@SentimentsNegative);" +
                "SELECT * FROM JobSearchProfiles WHERE Id = last_insert_rowid();";

            return await connection.QuerySingleAsync<JobSearchProfile>(sql, profile).ConfigureAwait(false);
        }

        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool columnFiltersEnabled,
            bool isAppliedTo,
            bool isInterviewing,
            bool isRejected,
            bool isFavourite)
        {
            if (columnFiltersEnabled)
            {
                const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden, " +
                "Notes FROM JobListings " +
                "WHERE IsAppliedTo = @IsAppliedTo " +
                "AND IsInterviewing = @IsInterviewing " +
                "AND IsRejected = @IsRejected " +
                "AND IsFavourite = @IsFavourite " +
                "AND IsHidden = False;";

                var jobListings = await connection.QueryAsync<JobListing>(
                sql,
                new
                {
                    IsAppliedTo = isAppliedTo,
                    IsInterviewing = isInterviewing,
                    IsRejected = isRejected,
                    IsFavourite = isFavourite
                }).ConfigureAwait(false);

                return jobListings.AsQueryable();
            }
            else
            {
                const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden, " +
                "Notes FROM JobListings " +
                "WHERE IsHidden = False;";

                var jobListings = await connection.QueryAsync<JobListing>(sql).ConfigureAwait(false);

                return jobListings.AsQueryable();
            }
        }

        public async Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync()
        {
            const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden " +
                "FROM JobListings " +
                "WHERE IsHidden = True " +
                "ORDER BY Id DESC;";

            return await connection.QueryAsync<JobListing>(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobListing>> GetFavouriteJobListingsAsync()
        {
            const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden " +
                "FROM JobListings " +
                "WHERE IsFavourite = True " +
                "ORDER BY Id DESC;";

            return await connection.QueryAsync<JobListing>(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListingsAsync()
        {
            const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden " +
                "FROM JobListings " +
                "WHERE IsHidden = False " +
                "ORDER BY Id DESC;";

            return await connection.QueryAsync<JobListing>(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobListing>> GetAllJobListingsWithFullDescriptionAsync()
        {
            const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsRejected, " +
                "IsFavourite, " +
                "IsHidden " +
                "FROM JobListings " +
                "WHERE IsHidden = False " +
                "ORDER BY Id DESC;";

            return await connection.QueryAsync<JobListing>(sql).ConfigureAwait(false);
        }

        public async Task<JobListing> GetJobListingDetailsByIdAsync(int id)
        {
            const string jobListingSQL = "SELECT Description, Notes From JobListings Where Id = @Id;";
            var jobListing = await connection.QuerySingleAsync<JobListing>(jobListingSQL, new { Id = id }).ConfigureAwait(false);

            const string applicationLinksSQL = "SELECT Link FROM ApplicationLinks Where JobListingId = @Id;";
            var applicationLinks = await connection.QueryAsync<string>(applicationLinksSQL, new { Id = id }).ConfigureAwait(false);

            if (applicationLinks != null && applicationLinks.Any())
            {
                StringBuilder sb = new();

                foreach (var link in applicationLinks)
                {
                    sb.AppendLine(link);
                    sb.AppendLine();
                }

                jobListing.ApplicationLinksString = sb.ToString();
            }

            return jobListing;
        }

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id)
        {
            string sql = $"UPDATE JobSearchProfiles SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}