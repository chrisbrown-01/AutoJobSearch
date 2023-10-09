using AutoJobSearchShared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    internal class DbContextSQLite
    {
        public static async Task UpdateDatabase(string columnName, bool value, int id)
        {
            // TODO: proper logging
            await SQLiteDb.UpdateDatabaseBoolPropertyById(columnName, value, id);
        }
    }
}
