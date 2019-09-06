using CoreApp.IServices;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public class UtilityService
    {
        public UtilityService()
        {
                
        }

        [AllowStaticCommand]
        public string GetAcademicYear(DateTime startDate, bool isWinter)
        {
            return startDate.TryGetAcademicYear(isWinter);
        }

        [AllowStaticCommand]
        public string GetAcademicYear(CoreApp.BusinessModels.SemesterUpdate semester)
        {
            return semester.StartDate.TryGetAcademicYear(semester.IsWinter);
        }

        [AllowStaticCommand]
        public bool GetSemester(CoreApp.BusinessModels.SemesterUpdate semester)
        {
            return semester.StartDate.TryGetSemesterType() ?? false; //error default(false = winter) 
        }


        public void GetSemesterUpdates(CoreApp.BusinessModels.SemesterCreate semester)
        {
            semester.IsWinter = semester.StartDate.TryGetSemesterType() ?? false;
            semester.AcademicYear = semester.StartDate.TryGetAcademicYear(semester.IsWinter);
        }
    }
}
