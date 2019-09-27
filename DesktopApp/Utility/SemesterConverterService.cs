using CoreApp.IServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Utility
{
    public static class SemesterConverterService
    {
        public static string GetAcademicYear(DateTime startDate)
        {
            return startDate.TryGetAcademicYear();
        }

        public static string GetAcademicYear(DateTime startDate, bool isWinter)
        {
            return startDate.TryGetAcademicYear(isWinter);
        }

        public static string GetAcademicYear(CoreApp.BusinessModels.SemesterUpdate semester)
        {
            return semester.StartDate.TryGetAcademicYear(semester.IsWinter);
        }

        public static bool GetSemester(CoreApp.BusinessModels.SemesterUpdate semester)
        {
            return semester.StartDate.TryGetSemesterType() ?? false; //error default(false = winter) 
        }

        public static bool GetSemester(DateTime startDate)
        {
            return startDate.TryGetSemesterType() ?? false;
        }


        public static void GetSemesterUpdates(CoreApp.BusinessModels.SemesterCreate semester)
        {
            semester.IsWinter = semester.StartDate.TryGetSemesterType() ?? false;
            semester.AcademicYear = semester.StartDate.TryGetAcademicYear(semester.IsWinter);
        }
    }
}
