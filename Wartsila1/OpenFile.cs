using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Wartsila1
{
    public class OpenFileClass
    {
        /*** 
         * Using FileOpenPicker class to open files. Based on sample code from 
         * Microsoft docs.
         ***/
        public async Task<IStorageFile> OpenFileAsync()
        {
            try
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
            catch (Exception ex)
            {
                return null; // This is to handle the cancel situation
            }
        }
    }
}
