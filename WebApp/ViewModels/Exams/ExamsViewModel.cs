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

namespace WebApp.ViewModels.Exams
{
    public class ExamsViewModel : MasterPageViewModel
    {
        private readonly IExamService _examService;
        private readonly IStudentExamService _studentExamService;
        private readonly ICourseService _courseService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly ISemesterService _semesterService;
        private readonly IExportService _exportService;

        public ExamsViewModel(
            IExamService examService,
            ICourseService courseService,
            IEnrolmentService enrolmentService,
            IStudentExamService studentExamService,
            ISemesterService semesterService,
            IExportService exportService)
        {
            _examService = examService;
            _courseService = courseService;
            _enrolmentService = enrolmentService;
            _studentExamService = studentExamService;
            _semesterService = semesterService;
            _exportService = exportService;
        }

        public GridViewDataSet<ExamListModel> Exams { get; set; } = new GridViewDataSet<ExamListModel>() { RowEditOptions = { PrimaryKeyPropertyName = "Id" } };

        public ExamCreate NewExam { get; set; }
        public StudentExamsCreateModel StudentExamsSelected { get; set; }

        public List<StudentExamUpdateListModel> StudentExams { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<GradeList> GradesList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<ExamTypeList> ExamTypesList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<CourseInstanceList> CourseInstancesList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<EnrolmentList> EnrolmentsList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<EnrolmentList> EnrolmentsListUpdate { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<string> ValidationErrors { get; set; }

        public string CurrentStudentExams { get; set; }
        public bool ShowCurrentStudentExams { get; set; } = false;

        [Bind(Direction.ServerToClient)]
        public List<CourseList> CoursesList { get; set; }
        [Bind(Direction.ServerToClient)]
        public List<SemesterList> SemestersList { get; set; }

        public int? CourseId { get; set; }
        public int? SemesterId { get; set; }
        public string FileName { get; set; } = "ispiti.xlsx";

        [Bind(Direction.ServerToClient)]
        public string ModalMessage { get; set; } = null;

        public override async Task Load()
        {
            CourseInstancesList = await _courseService.GetInastancesList();

            GradesList = Enum.GetValues(typeof(Grade)).OfType<Grade>().ToList().Select(_ => new GradeList
            {
                Grade = _,
                Description = _.GetEnumDescription()
            }).ToList();

            ExamTypesList = Enum.GetValues(typeof(ExamType)).OfType<ExamType>().ToList().Select(_ => new ExamTypeList
            {
                ExamType = _,
                Description = _.GetEnumDescription()
            }).ToList();

            await base.Load();
        }

        public override async Task PreRender()
        {
            if (Exams.IsRefreshRequired)
            {
                var examsList = (await _examService.GetUpdateable()).Select(_ => new ExamListModel
                {
                    Id = _.Id,
                    Course = _.Course,
                    Exam = _.Exam,
                    Semester = _.Semester,
                    Type = _.Type,
                    ExamTypeDescription = _.Type.GetEnumDescription()
                }) ;

                Exams.LoadFromQueryable(examsList.AsQueryable());
            }

            await base.PreRender();
        }

        #region Edit exam methods
        public void EditExam(ExamUpdate exam)
        {
            Exams.RowEditOptions.EditRowId = exam.Id;
        }

        public void CancelEditExam()
        {
            Exams.RowEditOptions.EditRowId = null;
            Exams.RequestRefresh();
        }

        public async Task SaveEditExam(ExamUpdate exam)
        {
            ValidationErrors = null;
            try
            {
                await _examService.Update(exam.Id, exam);
                Exams.RowEditOptions.EditRowId = null;
                Exams.RequestRefresh();
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }

        }
        #endregion

        #region Edit student exam methods
        public async Task SaveUpdateStudentExam(StudentExamUpdateListModel studentExam)
        {

            ValidationErrors = null;
            try
            {
                await _studentExamService.Update(studentExam.StudentExam);
                studentExam.Changed = false;
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }

        public async Task SaveAllUpdatesStudentExams()
        {
            ValidationErrors = null;
            try
            {
                await _studentExamService.Update(StudentExams.Select(_ => _.StudentExam).ToList());
                StudentExams.ForEach(_ => _.Changed = false);
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch(ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }
        #endregion

        #region Add exam methods
        public void AddExam()
        {
            NewExam = new ExamCreate () { Type = ExamType.Written };
        }

        public void CancelAddExam()
        {
            NewExam = null;
        }

        public async Task SaveAddExam()
        {
            ValidationErrors = null;
            try
            {
                await _examService.Create(NewExam);
                Exams.RowEditOptions.EditRowId = null;
                Exams.RequestRefresh();
                NewExam = null;
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }
        #endregion

        #region Add student exams to exam methods
        public async Task AddStudentsToExam(int examId)
        {
            var exam = Exams.Items.FirstOrDefault(_ => _.Id == examId);

            StudentExamsSelected = new StudentExamsCreateModel
            {
                ExamId = examId,
                ExamList = exam
            };

            await LoadForAddStudentsToExam(exam);
        }

        private async Task LoadForAddStudentsToExam(ExamListModel exam)
        {
            EnrolmentsListUpdate = await _enrolmentService.GetAvailableForCourseInstance(new CourseInstanceBase
            {
                CourseId = exam.Course.Id,
                SemesterId = exam.Semester.Id
            }, exam.Id);
        }

        public void CancelAddStudentsToExam()
        {
            StudentExamsSelected = null;
        }

        public async Task SaveAddStudentsToExam()
        {
            ValidationErrors = null;
            try
            {
                await _studentExamService.Create(StudentExamsSelected);
                //Exams.RowEditOptions.EditRowId = null;
                //Exams.RequestRefresh();
                await ExpandExam(StudentExamsSelected.ExamId);            
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
                await LoadForAddStudentsToExam(StudentExamsSelected.ExamList);
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
                await LoadForAddStudentsToExam(StudentExamsSelected.ExamList);
            }
        }
        #endregion

        #region Delete exam methods
        public async Task DeleteExam(ExamUpdateList exam)
        {
            ValidationErrors = null;
            try
            {
                await _examService.Delete(exam.Id);
                Exams.RowEditOptions.EditRowId = null;
                Exams.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }

        public async Task ForceDeleteExam(ExamUpdateList exam)
        {
            ValidationErrors = null;
            try
            {
                await _examService.ForceDelete(exam.Id);
                Exams.RowEditOptions.EditRowId = null;
                StudentExams = null;
                ShowCurrentStudentExams = false;
                Exams.RequestRefresh();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }
        #endregion

        #region Delete student exam methods
        public async Task RemoveStudentExam(StudentExamUpdateList studentExam)
        {
            ValidationErrors = null;
            try
            {
                await _studentExamService.Delete(studentExam.StudentExam.EnrolmentId, studentExam.StudentExam.ExamId);
                await ExpandExam(studentExam.StudentExam.ExamId);
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }
        #endregion

        public async Task ExpandExam(int examId)
        {
            StudentExams = (await _studentExamService.GetStudentExamsForExam(examId))
                .Select(_ => new StudentExamUpdateListModel
                {
                    Changed = false,
                    Student = _.Student,
                    StudentExam = _.StudentExam,
                    StudentExamId = _.StudentExamId
                }).ToList() ;

            var exam = Exams.Items.FirstOrDefault(_ => _.Id == examId);

            var date = exam.Exam.Date.HasValue ? exam.Exam.Date.Value.ToString("dd.MM.yyyy") : ""; 
            CurrentStudentExams = StudentExams.Count > 0 ?
                string.Format($"Studenti prijavljeni na ispit : {exam.ExamTypeDescription} {date} {exam.Course.Name} :") :
                string.Format($"Trenutno nema studenata prijavljenih na ispit : {exam.ExamTypeDescription} {date} {exam.Course.Name}");

            ShowCurrentStudentExams = true;

            CancelAddStudentsToExam();
        }

        public async Task GetEnrolments(CourseInstanceBase courseInstance)
        {
            EnrolmentsList = await _enrolmentService.GetForCourseInstance(courseInstance);
        }

        public async Task StartExport()
        {
            ModalMessage = null;
            CoursesList = await _courseService.GetList();
            SemestersList = await _semesterService.GetList();

            CourseId = CoursesList.FirstOrDefault()?.Id;
            SemesterId = SemestersList.FirstOrDefault()?.Id;

            Context.ResourceManager.AddStartupScript("$('div[data-id=export]').modal('show');");
        }

        public async Task GenerateFile(int? courseId, int? semesterId, string fileName)
        {
            if (!courseId.HasValue || !semesterId.HasValue)
            {
                ValidationErrors = new List<string> { "Nevaljali podaci za export." };
            }

            var data = await _studentExamService.GetStudentExamsExport(courseId.Value, semesterId.Value);

            if (data == null)
            {
                ModalMessage = "Nema studentskih ispita za odabrani predmet i semestar.";
            }
            else
            {
                var file = _exportService.ExportStudentExams(data, FileName);
                Context.ResourceManager.AddStartupScript("$('div[data-id=export]').modal('hide');");

                Context.ReturnFile(file.Content, file.Name, file.FileType);
            }
        }
    }
}
