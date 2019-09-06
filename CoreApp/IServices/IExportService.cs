using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApp.IServices
{
    public interface IExportService
    {
        File ExportStudentExams(StudentExamsExport data, string fileName = null);
    }
}
