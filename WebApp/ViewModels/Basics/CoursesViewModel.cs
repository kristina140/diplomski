using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.ViewModels.Basics
{
    public class CoursesViewModel : MasterPageViewModel
    {
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;

        public CoursesViewModel(ICourseService courseService, ISemesterService semesterService)
        {
            _courseService = courseService;
            _semesterService = semesterService;
        }

        public GridViewDataSet<CourseUpdateModel> Courses { get; set; } = new GridViewDataSet<CourseUpdateModel>() { RowEditOptions = { PrimaryKeyPropertyName = "Id" } };
        public CourseCreate NewCourse { get; set; }

        public Dictionary<int, bool> ToggleList { get; set; }

        [Bind(Direction.ServerToClient)]
        public IEnumerable<string> ValidationErrors { get; set; }

        //public override async Task Init()
        //{
        //    var coursesList = await _courseService.GetUpdateableModels();
        //    ToggleList = coursesList.ToDictionary(_ => _.Id, _ => false);

        //    await base.Init();
        //}

        public override async Task PreRender()
        {
            if (Courses.IsRefreshRequired)
            {
                var coursesList = await _courseService.GetUpdateableModels();
                ToggleList = coursesList.ToDictionary(_ => _.Id, _ => false);

                Courses.LoadFromQueryable(coursesList.AsQueryable());
            }

            await base.PreRender();
        }

        #region Toggle
        public void ToggleInstances(int courseId)
        {
            if (!ToggleList.ContainsKey(courseId))
                ToggleList.Add(courseId, false);

            ToggleList[courseId] = !ToggleList[courseId];
        }
        #endregion

        #region Edit methods
        public void Edit(CourseUpdateModel course)
        {
            Courses.RowEditOptions.EditRowId = course.Id;
        }

        public void CancelEdit()
        {
            Courses.RowEditOptions.EditRowId = null;
            Courses.RequestRefresh();
        }

        public async Task SaveEdit(CourseUpdateModel course)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.UpdateBasic(course.Id, course.Course);
                Courses.RowEditOptions.EditRowId = null;
                Courses.RequestRefresh();
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

        #region Add methods
        public void Add()
        {
            NewCourse = new CourseCreate { };
        }

        public void CancelAdd()
        {
            NewCourse = null;
        }

        public async Task Save()
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateBasic(NewCourse);
                Courses.RowEditOptions.EditRowId = null;
                Courses.RequestRefresh();
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

        #region Delete methods
        public async Task DeleteCourse(CourseUpdateModel course)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.Delete(course.Id);
                //CourseInstances.RowEditOptions.EditRowId = null;
                Courses.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }

        public async Task DeleteCourseWithInstances(CourseUpdateModel course)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.DeleteWithInstances(course.Id);
                Courses.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion

    }
}
