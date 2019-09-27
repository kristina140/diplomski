using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Utility
{
    public class DownloadFileService : IDownloadFileService
    {
        public void Download(CoreApp.BusinessModels.File FileToDownload)
        {
            if (FileToDownload != null)
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.FileName = "ispiti.xlsx";
                fileDialog.DefaultExt = ".xlsx";
                fileDialog.AddExtension = true;

                if (fileDialog.ShowDialog() == true)
                {
                    System.IO.File.WriteAllBytes(fileDialog.FileName, FileToDownload.Content);
                }
            }
        }
    }
}
