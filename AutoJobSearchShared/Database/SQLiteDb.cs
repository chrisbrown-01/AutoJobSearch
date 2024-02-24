using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        // TODO: add NOT NULL constraints to tables
        public void CreateTables()
        {
            connection.Execute("PRAGMA foreign_keys = ON;");

            const string createJobListingsTableSQL = "CREATE TABLE JobListings (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "SearchTerm TEXT," +
                "CreatedAt TEXT," +
                "StatusModifiedAt TEXT," +
                "Description_Raw TEXT," +
                "Description TEXT," +
                "Notes TEXT," +
                "Score INTEGER," +
                "IsToBeAppliedTo INTEGER," +
                "IsAppliedTo INTEGER," +
                "IsInterviewing INTEGER," +
                "IsNegotiating INTEGER," +
                "IsRejected INTEGER," +
                "IsDeclinedOffer INTEGER," +
                "IsAcceptedOffer INTEGER," +
                "IsFavourite INTEGER," +
                "IsHidden INTEGER)";

            connection.Execute(createJobListingsTableSQL);

            const string createApplicationLinksTableSQL = "CREATE TABLE ApplicationLinks (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "JobListingId INTEGER NOT NULL," +
                 "Link TEXT," +
                 "Link_RawHTML TEXT," +
                 "FOREIGN KEY(JobListingId) REFERENCES JobListings(Id) ON DELETE CASCADE)";

            connection.Execute(createApplicationLinksTableSQL);

            const string createJobSearchProfilesTableSQL = "CREATE TABLE JobSearchProfiles (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "MaxJobListingIndex INTEGER," +
                 "ProfileName TEXT," +
                 "Searches TEXT," +
                 "KeywordsPositive TEXT," +
                 "KeywordsNegative TEXT," +
                 "SentimentsPositive TEXT," +
                 "SentimentsNegative TEXT)";

            connection.Execute(createJobSearchProfilesTableSQL);

            const string createContactsTableSQL = "CREATE TABLE Contacts (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "CreatedAt TEXT," +
                "Company TEXT," +
                "Location TEXT," +
                "Name TEXT," +
                "Title TEXT," +
                "Email TEXT," +
                "Phone TEXT," +
                "LinkedIn TEXT," +
                "Notes TEXT)";

            connection.Execute(createContactsTableSQL);

            const string createContactsAssociatedJobIdTableSQL = "CREATE TABLE ContactsAssociatedJobIds (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ContactId INTEGER NOT NULL," +
                "JobListingId INTEGER NOT NULL," +
                "FOREIGN KEY (ContactId) REFERENCES Contacts(Id) ON DELETE CASCADE," +
                "FOREIGN KEY (JobListingId) REFERENCES JobListings(Id) ON DELETE CASCADE)";

            connection.Execute(createContactsAssociatedJobIdTableSQL);
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

        // TODO: test
        public async Task SaveJobListingsAsync(IEnumerable<JobListing> jobListings)
        {
            const string insertJobListingSQL =
                "INSERT INTO JobListings (" +
                "SearchTerm, " +
                "CreatedAt, " +
                "StatusModifiedAt, " +
                "Description_Raw, " +
                "Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
                "IsFavourite," +
                "IsHidden, " +
                "Notes" +
                ") VALUES (" +
                "@SearchTerm, " +
                "@CreatedAt, " +
                "@StatusModifiedAt, " +
                "@Description_Raw, " +
                "@Description, " +
                "@Score, " +
                "@IsToBeAppliedTo, " +
                "@IsAppliedTo, " +
                "@IsInterviewing, " +
                "@IsNegotiating, " +
                "@IsRejected, " +
                "@IsDeclinedOffer, " +
                "@IsAcceptedOffer, " +
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
                "MaxJobListingIndex, " +
                "ProfileName, " +
                "Searches, " +
                "KeywordsPositive, " +
                "KeywordsNegative, " +
                "SentimentsPositive, " +
                "SentimentsNegative" +
                ") VALUES (" +
                "@MaxJobListingIndex, " +
                "@ProfileName, " +
                "@Searches, " +
                "@KeywordsPositive, " +
                "@KeywordsNegative, " +
                "@SentimentsPositive, " +
                "@SentimentsNegative);" +
                "SELECT * FROM JobSearchProfiles WHERE Id = last_insert_rowid();";

            return await connection.QuerySingleAsync<JobSearchProfile>(sql, profile).ConfigureAwait(false);
        }

        // TODO: test
        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool columnFiltersEnabled,
            bool isToBeAppliedTo,
            bool isAppliedTo,
            bool isInterviewing,
            bool isNegotiating,
            bool isRejected,
            bool isDeclinedOffer,
            bool isAcceptedOffer,
            bool isFavourite)
        {
            if (columnFiltersEnabled)
            {
                const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "StatusModifiedAt, " +
                "Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
                "IsFavourite, " +
                "IsHidden, " +
                "Notes FROM JobListings " +
                "WHERE IsAppliedTo = @IsAppliedTo " +
                "AND IsToBeAppliedTo = @IsInterviewing " +
                "AND IsInterviewing = @IsInterviewing " +
                "AND IsNegotiating = @IsNegotiating " +
                "AND IsRejected = @IsRejected " +
                "AND IsDeclinedOffer = @IsDeclinedOffer " +
                "AND IsAcceptedOffer = @IsAcceptedOffer " +
                "AND IsFavourite = @IsFavourite " +
                "AND IsHidden = False;";

                var jobListings = await connection.QueryAsync<JobListing>(
                sql,
                new
                {
                    IsToBeAppliedTo = isToBeAppliedTo,
                    IsAppliedTo = isAppliedTo,
                    IsInterviewing = isInterviewing,
                    IsNegotiating = isNegotiating,
                    IsRejected = isRejected,
                    IsDeclinedOffer = isDeclinedOffer,
                    IsAcceptedOffer = isAcceptedOffer,
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
                "StatusModifiedAt, " +
                "Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
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
                "StatusModifiedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
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
                "StatusModifiedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
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
                "StatusModifiedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
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

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value, StatusModifiedAt = @StatusModifiedAt WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id, StatusModifiedAt = statusModifiedAt }).ConfigureAwait(false);
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

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            const string sql = "SELECT * FROM Contacts";
            return await connection.QueryAsync<Contact>(sql).ConfigureAwait(false);
        }

        // TODO: rename CreateNewContact to CreateContact
        public async Task<Contact> CreateNewContactAsync(Contact contact)
        {
            const string sql = 
                "INSERT INTO Contacts (" +
                "CreatedAt, " +
                "Company, " +
                "Location, " +
                "Name, " +
                "Title, " +
                "Email, " +
                "Phone, " +
                "LinkedIn, " +
                "Notes" +
                ") VALUES (" +
                "@CreatedAt, " +
                "@Company, " +
                "@Location, " +
                "@Name, " +
                "@Title, " +
                "@Email, " +
                "@Phone, " +
                "@LinkedIn, " +
                "@Notes);" +
                "SELECT * FROM Contacts WHERE Id = last_insert_rowid();";

            return await connection.QuerySingleAsync<Contact>(sql, contact).ConfigureAwait(false);
        }

        public async Task DeleteContactAsync(int id)
        {
            const string sql = "DELETE FROM Contacts WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id)
        {
            string sql = $"UPDATE Contacts SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task DeleteAllContactsAsync()
        {
            const string sql = "DELETE FROM Contacts; DELETE FROM ContactsAssociatedJobIds;";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync()
        {
            const string sql = "SELECT * FROM ContactsAssociatedJobIds";
            return await connection.QueryAsync<ContactAssociatedJobId>(sql).ConfigureAwait(false);
        }

        public async Task<ContactAssociatedJobId> CreateContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            const string sql =
                "INSERT INTO ContactsAssociatedJobIds (" +
                "ContactId, " +
                "JobListingId " +
                ") VALUES (" +
                "@ContactId, " +
                "@JobListingId);" +
                "SELECT * FROM ContactsAssociatedJobIds WHERE Id = last_insert_rowid();";

            return await connection.QuerySingleAsync<ContactAssociatedJobId>(sql, new { ContactId = contactId, JobListingId = jobId }).ConfigureAwait(false);
        }

        public async Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            const string sql = "DELETE FROM ContactsAssociatedJobIds WHERE ContactId = @ContactId AND JobListingId = @JobListingId;";
            await connection.ExecuteAsync(sql, new { ContactId = contactId, JobListingId = jobId }).ConfigureAwait(false);
        }

        public async Task DeleteJobAsync(int jobId)
        {
            const string sql = "DELETE FROM JobListings WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = jobId }).ConfigureAwait(false);
        }

        public async Task<JobListing> CreateJobAsync()
        {
            var newJob = new JobListing();

            const string sql = "INSERT INTO JobListings (" +
                "SearchTerm, " +
                "CreatedAt, " +
                "StatusModifiedAt, " +
                "Description_Raw, " +
                "Description, " +
                "Score, " +
                "IsToBeAppliedTo, " +
                "IsAppliedTo, " +
                "IsInterviewing, " +
                "IsNegotiating, " +
                "IsRejected, " +
                "IsDeclinedOffer, " +
                "IsAcceptedOffer, " +
                "IsFavourite, " +
                "IsHidden, " +
                "Notes) VALUES (" +
                "@SearchTerm, " +
                "@CreatedAt, " +
                "@StatusModifiedAt, " +
                "@Description_Raw, " +
                "@Description, " +
                "@Score, " +
                "@IsToBeAppliedTo, " +
                "@IsAppliedTo, " +
                "@IsInterviewing, " +
                "@IsNegotiating, " +
                "@IsRejected, " +
                "@IsDeclinedOffer, " +
                "@IsAcceptedOffer, " +
                "@IsFavourite, " +
                "@IsHidden, " +
                "@Notes);" +
                "SELECT * FROM JobListings WHERE Id = last_insert_rowid();";

            return await connection.QuerySingleAsync<JobListing>(
                sql, 
                new 
                { 
                    SearchTerm = newJob.SearchTerm, 
                    CreatedAt = newJob.CreatedAt,
                    StatusModifiedAt = newJob.StatusModifiedAt,
                    Description_Raw = newJob.Description_Raw,
                    Description = newJob.Description,
                    Score = newJob.Score,
                    IsToBeAppliedTo = newJob.IsToBeAppliedTo,
                    IsAppliedTo = newJob.IsAppliedTo,
                    IsInterviewing = newJob.IsInterviewing,
                    IsNegotiating = newJob.IsNegotiating,
                    IsRejected = newJob.IsRejected,
                    IsDeclinedOffer = newJob.IsDeclinedOffer,
                    IsAcceptedOffer = newJob.IsAcceptedOffer,
                    IsFavourite = newJob.IsFavourite,
                    IsHidden = newJob.IsHidden,
                    Notes = newJob.Notes
                }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id)
        {
            string sql = $"UPDATE JobSearchProfiles SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }
    }
}