using AutoJobSearchGUI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.ViewModels
{
    public partial class ContactsViewModel : ViewModelBase
    {
        private readonly IDbContext _dbContext;

        public ContactsViewModel(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
