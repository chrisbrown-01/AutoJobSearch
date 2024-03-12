using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Services
{
    internal interface IFilesService
    {
        public Task<IStorageFile?> OpenFileAsync();

        public Task<IStorageFile?> SaveFileAsync();
    }
}