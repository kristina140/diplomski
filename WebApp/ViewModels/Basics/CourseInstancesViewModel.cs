using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Services;

namespace WebApp.ViewModels.Basics
{
    public class CourseInstancesViewModel : MasterPageViewModel
    {
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;
        private readonly UtilityService _utilityService;

        public CourseInstancesViewModel(ICourseService courseService, ISemesterService semesterService, UtilityService utilityService)
        {
            _courseService = courseService;
            _semesterService = semesterService;
            _utilityService = utilityService;
        }

        public GridViewDataSet<CourseInstanceUpdate> CourseInstances { get; set; } = new GridViewDataSet<CourseInstanceUpdate>() { };
        public CourseInstanceCreate NewInstance { get; set; }

        public CourseCreate NewCourse { get; set; }
        public SemesterCreate NewSemester { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<CourseList> CoursesList { get; set; }
        [Bind(Direction.ServerToClient)]
        public List<SemesterList> SemestersList { get; set; }

        [Bind(Direction.ServerToClient)]
        public IEnumerable<string> ValidationErrors { get; set; }

        public override async Task PreRender()
        {
            if (CourseInstances.IsRefreshRequired)
            {
                var courseInstancesList = await _courseService.GetInstancesUpdateable();
                courseInstancesList.ForEach(_ => _.Semester.AcademicYear = _utilityService.GetAcademicYear(_.Semester.StartDate, _.Semester.IsWinter));
                CourseInstances.LoadFromQueryable(courseInstancesList.AsQueryable());
            }

            CoursesList = await _courseService.GetList();
            SemestersList = await _semesterService.GetList();

            await base.PreRender();
        }


        //#region Edit methods
        //public void Edit(CourseInstanceUpdate course)
        //{
        //    CourseInstances.RowEditOptions.EditRowId = course.Id;
        //}

        //public void CancelEdit()
        //{
        //    CourseInstances.RowEditOptions.EditRowId = null;
        //    CourseInstances.RequestRefresh();
        //}

        //public async Task SaveEdit(CourseInstanceUpdate course)
        //{
        //    ValidationErrors = null;
        //    try
        //    {
        //        await _courseService.UpdateBasic(course.Id, course);
        //        CourseInstances.RowEditOptions.EditRowId = null;
        //        CourseInstances.RequestRefresh();
        //    }
        //    catch (ValidationPropertyException vpe)
        //    {
        //        ValidationErrors = vpe.ErrorsList;
        //    }
        //    catch (ValidationException ve)
        //    {
        //        ValidationErrors = ve.Errors;
        //    }

        //}
        //#endregion


        #region Add instance methods
        public void AddInstance()
        {
            NewInstance = new CourseInstanceCreate { CourseId = 0, SemesterId = 0 };
        }

        public void CancelAdd()
        {
            NewInstance = null;
            NewCourse = null;
            NewSemester = null;
        }

        public async Task SaveInstance()
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateInstance(NewInstance.CourseId, NewInstance.SemesterId);
                //CourseInstances.RowEditOptions.EditRowId = null;
                CourseInstances.RequestRefresh();
                NewInstance = null;
                NewCourse = null;
                NewSemester = null;
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList;
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion

        #region Add course methods
        public void AddCourse()
        {
            NewCourse = new CourseCreate {};
        }

        public void CancelAddCourse()
        {
            NewCourse = null;
        }

        public async Task SaveCourse()
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateBasic(NewCourse);
                NewCourse = null;
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList;
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion

        #region Add semester methods
        public void AddSemester()
        {
            NewSemester = new SemesterCreate { StartDate = DateTime.Now };
        }

        public void CancelAddSemester()
        {
            NewSemester = null;
        }

        public async Task SaveSemester()
        {
            ValidationErrors = null;
            try
            {
                await _semesterService.Create(NewSemester);
                NewSemester = null;
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList;
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion

        #region Delete methods
        public async Task DeleteInstance(CourseInstanceUpdate instance)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.DeleteInstance(instance.Course.Id, instance.Semester.Id);
                //CourseInstances.RowEditOptions.EditRowId = null;
                CourseInstances.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }

        public async Task DeleteCourse(CourseInstanceUpdate instance)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.Delete(instance.Course.Id);
                //CourseInstances.RowEditOptions.EditRowId = null;
                CourseInstances.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }

        public async Task DeleteCourseWithInstances(CourseInstanceUpdate instance)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.DeleteWithInstances(instance.Course.Id);
                CourseInstances.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion

        #region Utility
        public void UpdateSemester(SemesterUpdate semester)
        {
            _utilityService.GetSemesterUpdates(semester);
        }

        public void UpdateSemester()
        {
            _utilityService.GetSemesterUpdates(NewSemester);
        }
        #endregion
    }
}
