using AutoJobSearchShared.Enums;
using AutoJobSearchShared.Models;
using Dapper;
using Microsoft.Data.Sqlite;
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

        public async Task<Contact> CreateContactAsync(Contact contact)
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

        public async Task CreateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles)
        {
            const string sql = "INSERT INTO JobListingsAssociatedFiles (" +
                "Id, " +
                "Resume, " +
                "CoverLetter, " +
                "File1, " +
                "File2, " +
                "File3)" +
                "VALUES (" +
                "@Id, " +
                "@Resume, " +
                "@CoverLetter, " +
                "@File1, " +
                "@File2, " +
                "@File3);";

            await connection.ExecuteAsync(sql,
                new
                {
                    Id = jobListingAssociatedFiles.Id,
                    Resume = jobListingAssociatedFiles.Resume,
                    CoverLetter = jobListingAssociatedFiles.CoverLetter,
                    File1 = jobListingAssociatedFiles.File1,
                    File2 = jobListingAssociatedFiles.File2,
                    File3 = jobListingAssociatedFiles.File3
                }).ConfigureAwait(false);
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

        // TODO: investigate creating an index on ApplicationLinks, ContactsAssociatedJobs table
        public void CreateTables()
        {
            connection.Execute("PRAGMA foreign_keys = ON;");

            const string createJobListingsTableSQL = "CREATE TABLE JobListings (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "SearchTerm TEXT NOT NULL," +
                "CreatedAt TEXT NOT NULL," +
                "StatusModifiedAt TEXT NOT NULL," +
                "Description_Raw TEXT NOT NULL," +
                "Description TEXT NOT NULL," +
                "Notes TEXT NOT NULL," +
                "Score INTEGER NOT NULL," +
                "IsToBeAppliedTo INTEGER NOT NULL," +
                "IsAppliedTo INTEGER NOT NULL," +
                "IsInterviewing INTEGER NOT NULL," +
                "IsNegotiating INTEGER NOT NULL," +
                "IsRejected INTEGER NOT NULL," +
                "IsDeclinedOffer INTEGER NOT NULL," +
                "IsAcceptedOffer INTEGER NOT NULL," +
                "IsFavourite INTEGER NOT NULL," +
                "IsHidden INTEGER NOT NULL)";

            connection.Execute(createJobListingsTableSQL);

            const string createApplicationLinksTableSQL = "CREATE TABLE ApplicationLinks (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "JobListingId INTEGER NOT NULL," +
                 "Link TEXT NOT NULL," +
                 "Link_RawHTML TEXT NOT NULL," +
                 "FOREIGN KEY(JobListingId) REFERENCES JobListings(Id) ON DELETE CASCADE)";

            connection.Execute(createApplicationLinksTableSQL);

            const string createJobListingsAssociatedFilesTableSQL = "CREATE TABLE JobListingsAssociatedFiles (" +
                "Id INTEGER PRIMARY KEY," +
                "Resume TEXT NOT NULL," +
                "CoverLetter TEXT NOT NULL," +
                "File1 TEXT NOT NULL," +
                "File2 TEXT NOT NULL," +
                "File3 TEXT NOT NULL," +
                "FOREIGN KEY (Id) REFERENCES JobListings (Id) ON DELETE CASCADE)";

            connection.Execute(createJobListingsAssociatedFilesTableSQL);

            const string createJobSearchProfilesTableSQL = "CREATE TABLE JobSearchProfiles (" +
                 "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                 "MaxJobListingIndex INTEGER NOT NULL," +
                 "ProfileName TEXT NOT NULL," +
                 "Searches TEXT NOT NULL," +
                 "KeywordsPositive TEXT NOT NULL," +
                 "KeywordsNegative TEXT NOT NULL," +
                 "SentimentsPositive TEXT NOT NULL," +
                 "SentimentsNegative TEXT NOT NULL)";

            connection.Execute(createJobSearchProfilesTableSQL);

            const string createContactsTableSQL = "CREATE TABLE Contacts (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "CreatedAt TEXT NOT NULL," +
                "Company TEXT NOT NULL," +
                "Location TEXT NOT NULL," +
                "Name TEXT NOT NULL," +
                "Title TEXT NOT NULL," +
                "Email TEXT NOT NULL," +
                "Phone TEXT NOT NULL," +
                "LinkedIn TEXT NOT NULL," +
                "Notes TEXT NOT NULL)";

            connection.Execute(createContactsTableSQL);

            const string createContactsAssociatedJobIdTableSQL = "CREATE TABLE ContactsAssociatedJobIds (" +
                "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ContactId INTEGER NOT NULL," +
                "JobListingId INTEGER NOT NULL," +
                "FOREIGN KEY (ContactId) REFERENCES Contacts(Id) ON DELETE CASCADE," +
                "FOREIGN KEY (JobListingId) REFERENCES JobListings(Id) ON DELETE CASCADE)";

            connection.Execute(createContactsAssociatedJobIdTableSQL);
        }

        public async Task DeleteAllContactsAsync()
        {
            const string sql = "DELETE FROM Contacts; DELETE FROM ContactsAssociatedJobIds;";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }

        public async Task DeleteAllJobListingsAsync()
        {
            const string sql = "DELETE FROM ApplicationLinks; DELETE FROM JobListings;";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }

        public async Task DeleteContactAssociatedJobIdAsync(int contactId, int jobId)
        {
            const string sql = "DELETE FROM ContactsAssociatedJobIds WHERE ContactId = @ContactId AND JobListingId = @JobListingId;";
            await connection.ExecuteAsync(sql, new { ContactId = contactId, JobListingId = jobId }).ConfigureAwait(false);
        }

        public async Task DeleteContactAsync(int id)
        {
            const string sql = "DELETE FROM Contacts WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        }

        public async Task DeleteJobAsync(int jobId)
        {
            const string sql = "DELETE FROM JobListings WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = jobId }).ConfigureAwait(false);
        }

        public async Task DeleteJobSearchProfileAsync(int id)
        {
            const string sql = "DELETE FROM JobSearchProfiles WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        public async Task<IQueryable<JobListing>> ExecuteJobListingQueryAsync(
            bool descriptionFilterEnabled,
            bool notesFilterEnabled,
            bool columnFilterEnabled,
            bool isToBeAppliedTo,
            bool isAppliedTo,
            bool isInterviewing,
            bool isNegotiating,
            bool isRejected,
            bool isDeclinedOffer,
            bool isAcceptedOffer,
            bool isFavourite)
        {
            string descriptionSQL;
            string notesSQL;
            string columnSQL;

            if (descriptionFilterEnabled)
            {
                descriptionSQL = "Description";
            }
            else
            {
                descriptionSQL = "SUBSTR(Description, 1, 200) AS Description";
            }

            if (notesFilterEnabled)
            {
                notesSQL = "Notes";
            }
            else
            {
                notesSQL = "SUBSTR(Notes, 1, 0) AS Notes";
            }

            if (columnFilterEnabled)
            {
                columnSQL = $"IsToBeAppliedTo = {isToBeAppliedTo} "
                    + $"AND IsAppliedTo = {isAppliedTo} "
                    + $"AND IsInterviewing = {isInterviewing} "
                    + $"AND IsNegotiating = {isNegotiating} "
                    + $"AND IsRejected = {isRejected} "
                    + $"AND IsDeclinedOffer = {isDeclinedOffer} "
                    + $"AND IsAcceptedOffer = {isAcceptedOffer} "
                    + $"AND IsFavourite = {isFavourite} "
                    + "AND IsHidden = False";
            }
            else
            {
                columnSQL = "IsHidden = False";
            }

            string querySQL =
                $@"SELECT
                    Q1.Id,
                    Q1.SearchTerm,
                    Q1.CreatedAt,
                    Q1.StatusModifiedAt,
                    Q1.Score,
                    Q1.IsAppliedTo,
                    Q1.IsToBeAppliedTo,
                    Q1.IsInterviewing,
                    Q1.IsNegotiating,
                    Q1.IsRejected,
                    Q1.IsDeclinedOffer,
                    Q1.IsAcceptedOffer,
                    Q1.IsFavourite,
                    Q1.IsHidden,
                    Q2.Description,
                    Q3.Notes
                FROM
                    (SELECT
                        Id,
                        SearchTerm,
                        CreatedAt,
                        StatusModifiedAt,
                        Score,
                        IsAppliedTo,
                        IsToBeAppliedTo,
                        IsInterviewing,
                        IsNegotiating,
                        IsRejected,
                        IsDeclinedOffer,
                        IsAcceptedOffer,
                        IsFavourite,
                        IsHidden
                    FROM
                        JobListings
                    WHERE
                        {columnSQL}) AS Q1
                JOIN
                    (SELECT
                        Id,
                        {descriptionSQL}
                    FROM
                        JobListings
                    WHERE
                        IsHidden = False) AS Q2
                ON Q1.Id = Q2.Id
                JOIN
                    (SELECT
                        Id,
                        {notesSQL}
                    FROM
                        JobListings
                    WHERE
                        IsHidden = False) AS Q3
                ON Q1.Id = Q3.Id;";

            var jobListings = await connection.QueryAsync<JobListing>(querySQL).ConfigureAwait(false);

            return jobListings.AsQueryable();
        }

        public async Task<IEnumerable<string>> GetAllApplicationLinksAsync()
        {
            const string sql = "SELECT Link FROM ApplicationLinks;";
            return await connection.QueryAsync<string>(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<ContactAssociatedJobId>> GetAllContactsAssociatedJobIdsAsync()
        {
            const string sql = "SELECT * FROM ContactsAssociatedJobIds";
            return await connection.QueryAsync<ContactAssociatedJobId>(sql).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contact>> GetAllContactsAsync()
        {
            const string sql = "SELECT * FROM Contacts";
            return await connection.QueryAsync<Contact>(sql).ConfigureAwait(false);
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

        public async Task<IEnumerable<JobSearchProfile>> GetAllJobSearchProfilesAsync()
        {
            const string sql = "SELECT * FROM JobSearchProfiles";
            return await connection.QueryAsync<JobSearchProfile>(sql).ConfigureAwait(false);
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

        public async Task<IEnumerable<JobListing>> GetHiddenJobListingsAsync()
        {
            const string sql =
                "SELECT Id, " +
                "SearchTerm, " +
                "CreatedAt, " +
                "StatusModifiedAt, " +
                "SUBSTR(Description, 1, 200) AS Description, " +
                "Score, " +
                "IsAppliedTo, " +
                "IsToBeAppliedTo, " +
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

            const string associatedFilesSQL = "SELECT * FROM JobListingsAssociatedFiles WHERE Id = @Id;";
            var associatedFiles = await connection.QuerySingleOrDefaultAsync<JobListingAssociatedFiles>(associatedFilesSQL, new { Id = id }).ConfigureAwait(false);

            if (associatedFiles != null)
            {
                jobListing.JobListingAssociatedFiles = associatedFiles;
            }

            return jobListing;
        }

        public async Task<JobSearchProfile?> GetJobSearchProfileByIdAsync(int id)
        {
            const string sql = "SELECT * FROM JobSearchProfiles WHERE Id = @Id;";
            return await connection.QuerySingleOrDefaultAsync<JobSearchProfile>(sql, new { Id = id }).ConfigureAwait(false);
        }

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

        public async Task UpdateContactStringPropertyAsync(ContactStringField columnName, string value, int id)
        {
            string sql = $"UPDATE Contacts SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingAssociatedFilesAsync(JobListingAssociatedFiles jobListingAssociatedFiles)
        {
            const string sql = "UPDATE JobListingsAssociatedFiles SET " +
                "Resume = @Resume, " +
                "CoverLetter = @CoverLetter, " +
                "File1 = @File1, " +
                "File2 = @File2, " +
                "File3 = @File3 " +
                "WHERE Id = @Id";

            await connection.ExecuteAsync(sql,
                new
                {
                    Id = jobListingAssociatedFiles.Id,
                    Resume = jobListingAssociatedFiles.Resume,
                    CoverLetter = jobListingAssociatedFiles.CoverLetter,
                    File1 = jobListingAssociatedFiles.File1,
                    File2 = jobListingAssociatedFiles.File2,
                    File3 = jobListingAssociatedFiles.File3
                }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingBoolPropertyAsync(JobListingsBoolField columnName, bool value, int id, DateTime statusModifiedAt)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value, StatusModifiedAt = @StatusModifiedAt WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id, StatusModifiedAt = statusModifiedAt }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingIntPropertyAsync(JobListingsIntField columnName, int value, int id)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobListingStringPropertyAsync(JobListingsStringField columnName, string value, int id)
        {
            string sql = $"UPDATE JobListings SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobSearchProfileIntPropertyAsync(JobSearchProfilesIntField columnName, int value, int id)
        {
            string sql = $"UPDATE JobSearchProfiles SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }

        public async Task UpdateJobSearchProfileStringPropertyAsync(JobSearchProfilesStringField columnName, string value, int id)
        {
            string sql = $"UPDATE JobSearchProfiles SET {columnName} = @Value WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Value = value, Id = id }).ConfigureAwait(false);
        }
    }
}