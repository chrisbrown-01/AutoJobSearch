using AutoJobSearchGUI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Models
{
    internal class ViewStackModel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel">ViewModel enum type.</param>
        /// <param name="itemId">Value must equal the id of the model object being inserted onto the stack. Defaults to 1.</param>
        /// <exception cref="ArgumentException"></exception>
        public ViewStackModel(ViewModel viewModel, int itemId = 1)
        {
            ViewModel = viewModel;
            ItemId = itemId;
        }

        public ViewModel ViewModel { get; init; }

        public int ItemId { get; init; }
    }
}
