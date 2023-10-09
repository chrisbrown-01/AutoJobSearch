using AutoJobSearchShared.Enums;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Data
{
    internal interface IDbContext
    {
        Task UpdateJobListingBoolProperty(DbBoolField columnName, bool value, int id);
        Task UpdateJobListingStringProperty(DbStringField columnName, string value, int id);
    }
}