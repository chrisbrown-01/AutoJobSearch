using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    // If changing to a different database provider other than the default SQLite database, update the methods in this class.
    internal class DbContext // TODO: interface
    {
        public DbContext()
        {
            JobListingModel.BoolFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingBoolProperty(e.Field, e.Value, e.Id);
            };

            JobListingModel.StringFieldChanged += async (sender, e) =>
            {
                await UpdateJobListingStringProperty(e.Field, e.Value, e.Id);
            };
        }

        public async Task UpdateJobListingBoolProperty(DbBoolField columnName, bool value, int id)
        {
            // TODO: proper logging
            await SQLiteDb.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id)
        {
            // TODO: proper logging
            await SQLiteDb.UpdateJobListingStringProperty(columnName, value, id);
        }
    }
}
