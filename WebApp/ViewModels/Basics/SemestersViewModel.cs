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
    public class SemestersViewModel : MasterPageViewModel
    {
        private readonly ISemesterService _semesterService;
        private readonly UtilityService _utilityService;

        public SemestersViewModel(ISemesterService semesterService, UtilityService utilityService)
        {
            _semesterService = semesterService;
            _utilityService = utilityService;
        }

        public GridViewDataSet<SemesterUpdate> Semesters { get; set; } = new GridViewDataSet<SemesterUpdate>() { RowEditOptions = { PrimaryKeyPropertyName = "Id" } };
        //public List<SemesterCreate> NewSemesters { get; set; } = new List<SemesterCreate> { };
        public SemesterCreate NewSemester { get; set; }

        public string Dummy { get; set; }

        [Bind(Direction.ServerToClient)]
        public IEnumerable<string> ValidationErrors { get; set; }
        //[Bind(Direction.ServerToClient)]
        //public bool ShowSaveButton { get; set; } = false;

        public override async Task PreRender()
        {
            if (Semesters.IsRefreshRequired)
            {
                var semestersList = await _semesterService.GetUpdateable();
                semestersList.ForEach(_ => _.AcademicYear = _utilityService.GetAcademicYear(_.StartDate, _.IsWinter));
                Semesters.LoadFromQueryable(semestersList.AsQueryable());
            }

            //ShowSaveButton = NewSemesters.Any();

            await base.PreRender();
        }

        #region Edit methods
        public void Edit(SemesterUpdate semester)
        {
            Semesters.RowEditOptions.EditRowId = semester.Id;
        }

        public void CancelEdit()
        {
            Semesters.RowEditOptions.EditRowId = null;
            Semesters.RequestRefresh();
        }

        public async Task SaveEdit(SemesterUpdate semester)
        {
            ValidationErrors = null;
            try
            {
                await _semesterService.UpdateBasic(semester.Id, semester);
                Semesters.RowEditOptions.EditRowId = null;
                Semesters.RequestRefresh();
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
        public void AddSemester()
        {
            //var semester = new SemesterCreate { StartDate = DateTime.Now };
            //NewSemesters.Add(semester);

            NewSemester = new SemesterCreate { StartDate = DateTime.Now };
        }

        //public void CancelAdd(SemesterCreate semester)
        //{
        //    NewSemesters.Remove(semester);
        //}

        public void CancelAdd()
        {
            NewSemester = null;
        }

        public async Task Save()
        {
            ValidationErrors = null;
            try
            {
                //await _semesterService.Create(NewSemesters);
                await _semesterService.Create(NewSemester);
                Semesters.RowEditOptions.EditRowId = null;
                Semesters.RequestRefresh();
                //NewSemesters.Clear();
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
        public async Task Delete(SemesterUpdate semester)
        {
            ValidationErrors = null;
            try
            {
                await _semesterService.Delete(semester.Id);
                Semesters.RowEditOptions.EditRowId = null;
                Semesters.RequestRefresh();
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
