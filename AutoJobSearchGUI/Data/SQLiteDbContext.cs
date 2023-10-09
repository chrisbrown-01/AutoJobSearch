using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    internal class SQLiteDbContext : IDbContext
    {
        public async Task UpdateJobListingBoolProperty(DbBoolField columnName, bool value, int id)
        {
            await SQLiteDb.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id)
        {
            await SQLiteDb.UpdateJobListingStringProperty(columnName, value, id);
        }
    }
}
