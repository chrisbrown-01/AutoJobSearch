﻿using AutoJobSearchGUI.Models;
using AutoJobSearchShared;
using AutoJobSearchShared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    internal class DbContext : IDbContext
    {
        private readonly IDbContext _dbContext;

        public DbContext()
        {
            _dbContext = new SQLiteDbContext();

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
            await _dbContext.UpdateJobListingBoolProperty(columnName, value, id);
        }

        public async Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id)
        {
            // TODO: proper logging
            await _dbContext.UpdateJobListingStringProperty(columnName, value, id);
        }
    }
}
