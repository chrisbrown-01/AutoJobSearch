using AutoJobSearchGUI.Data;
using AutoJobSearchGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class AddContactViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public AddContactViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // TODO: ensure that undo action in text boxes reflects in the SQLite database events and writing
        // TODO: create tests
    }
}
