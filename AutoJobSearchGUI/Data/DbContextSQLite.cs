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
        public static void UpdateDatabase(string columnName, bool value, int id) // TODO: async Task?
        {
            // TODO: proper logging
            // Debug.WriteLine("Updating database"); // TODO: proper logging
            SQLiteDb.UpdateDatabaseBoolPropertyById(columnName, value, id);
        }
    }
}
