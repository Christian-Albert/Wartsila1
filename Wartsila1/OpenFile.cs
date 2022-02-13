using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Wartsila1
{
    // Based on sample code from Microsoft docs
    public class OpenFileClass
    {
        public async Task<IStorageFile> OpenFileAsync()
        {
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary

            };
            openPicker.FileTypeFilter.Add(".txt");
            openPicker.FileTypeFilter.Add(".csv");

            return await openPicker.PickSingleFileAsync();
        }
    }
}
