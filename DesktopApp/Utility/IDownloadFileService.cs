using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Utility
{
    public interface IDownloadFileService
    {
        void Download(CoreApp.BusinessModels.File FileToDownload);
    }
}
