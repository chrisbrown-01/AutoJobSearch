using AutoJobSearchGUI.Enums;
using System;

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

        public int ItemId { get; init; }
        public ViewModel ViewModel { get; init; }
    }
}