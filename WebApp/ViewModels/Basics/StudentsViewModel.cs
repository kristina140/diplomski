using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;

namespace WebApp.ViewModels.Basics
{
    public class StudentsViewModel : MasterPageViewModel
    {
        private readonly IStudentService _studentService;

        public StudentsViewModel(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public GridViewDataSet<StudentUpdate> Students { get; set; } = new GridViewDataSet<StudentUpdate>() { RowEditOptions = { PrimaryKeyPropertyName = "Id" } };
        //public List<StudentCreate> NewStudents { get; set; } = new List<StudentCreate> { };
        public StudentCreate NewStudent { get; set; }

        [Bind(Direction.ServerToClient)]
        public IEnumerable<string> ValidationErrors { get; set; }
        //[Bind(Direction.ServerToClient)]
        //public bool ShowSaveButton { get; set; } = false;

        public override async Task PreRender()
        {
            if (Students.IsRefreshRequired)
            {
                var studentsList = await _studentService.GetUpdateable();
                Students.LoadFromQueryable(studentsList.AsQueryable());
            }

            //ShowSaveButton = NewStudents.Any();

            await base.PreRender();
        }

        #region Edit methods
        public void Edit(StudentUpdate student)
        {
            Students.RowEditOptions.EditRowId = student.Id;
        }

        public void CancelEdit()
        {
            Students.RowEditOptions.EditRowId = null;
            Students.RequestRefresh();
        }

        public async Task SaveEdit(StudentUpdate student)
        {
            ValidationErrors = null;
            try
            {
                await _studentService.UpdateBasic(student.Id, student);
                Students.RowEditOptions.EditRowId = null;
                Students.RequestRefresh();
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
        public void AddStudent()
        {
            //var student = new StudentCreate { };

            //int id = 1;
            //if (NewStudents.Any())
            //    id = NewStudents.Max(_ => _.Id) + 1;
            //else
            //    id = Students.Items.Max(_ => _.Id) + 1;
            //var student = new StudentUpdate { Id = id };
            //NewStudents.Add(student);

            NewStudent = new StudentCreate { };
        }

        //public void CancelAdd(StudentUpdate student)
        //{
        //    NewStudents.Remove(student);
        //}

        public void CancelAdd()
        {
            NewStudent = null;
        }

        public async Task Save()
        {
            ValidationErrors = null;
            try
            {
                //await _studentService.Create(NewStudents);
                await _studentService.Create(NewStudent);
                Students.RowEditOptions.EditRowId = null;
                Students.RequestRefresh();
                //NewStudents.Clear();
                NewStudent = null;
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
        public async Task Delete(StudentUpdate student)
        {
            ValidationErrors = null;
            try
            {
                await _studentService.Delete(student.Id);
                Students.RowEditOptions.EditRowId = null;
                Students.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors;
            }
        }
        #endregion
    }
}

