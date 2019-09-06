using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.ViewModels.Exams
{
    public class OralExamViewModel : MasterPageViewModel
    {
        private readonly IExamService _examService;
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IStudentService _studentService;
        private readonly IStudentExamService _studentExamService;

        public OralExamViewModel(
            IExamService examService,
            ICourseService courseService,
            ISemesterService semesterService,
            IEnrolmentService enrolmentService,
            IStudentService studentService,
            IStudentExamService studentExamService)
        {
            _examService = examService;
            _courseService = courseService;
            _semesterService = semesterService;
            _enrolmentService = enrolmentService;
            _studentService = studentService;
            _studentExamService = studentExamService;
        }

        public OralExamCreate StudentOralExam { get; set; } = new OralExamCreate() { };

        #region Courses
        [Bind(Direction.ServerToClient)]
        public List<CourseList> CoursesList { get; set; }
        public int SelectedCourseId { get; set; }
        #endregion

        #region Semesters
        [Bind(Direction.ServerToClient)]
        public List<SemesterList> SemestersList { get; set; }
        public int SelectedSemesterId { get; set; }
        #endregion

        public bool CourseInstanceExist { get; set; }

        #region Exams
        public bool IsExistingExams { get; set; } = true;
        public bool AnyExistingExams { get; set; } = false;

        [Bind(Direction.ServerToClient)]
        public List<ExamList> ExamsList { get; set; }

        public bool ExamDateTimeNotSet { get; set; } = false;
        public DateTime? ExamDate { get; set; } = DateTime.Now;
        public DateTime? ExamTime { get; set; } = DateTime.Now;
        #endregion

        #region Enrolments / students
        [Bind(Direction.ServerToClient)]
        public List<EnrolmentList> EnrolmentsList { get; set; } 

        public bool AddNewStudent { get; set; } = false;
        [Bind(Direction.ServerToClient)]
        public List<StudentBase> StudentsList { get; set; }
        public EnrolmentBaseCreate NewEnrolment { get; set; }
        #endregion

        [Bind(Direction.ServerToClient)]
        public List<string> ValidationErrors { get; set; }

        public string SearchEnrolment { get; set; }
        public string SearchStudent { get; set; }
        public StudentBase SelectedStudent { get; set; }

        //[Bind(Direction.ServerToClient)]
        public ExistingStudentOralExam ExistingStudentOralExam { get; set; } = null;

        public override Task Init()
        {
            return base.Init();
        }

        public override async Task Load()
        {
            CoursesList = await _courseService.GetList();
            SemestersList = await _semesterService.GetList();

            if (!Context.IsPostBack)
            {
                var course = CoursesList.FirstOrDefault();
                if (course != null)
                    SelectedCourseId = course.Id;

                var semester = SemestersList.FirstOrDefault();
                if (semester != null)
                    SelectedSemesterId = semester.Id;
            }

            CourseInstanceExist = await _courseService.CourseInstanceExists(SelectedCourseId, SelectedSemesterId);

            if (CourseInstanceExist)
            {
                ExamsList = await _examService.GetOralExamsListForCourseInstance(SelectedCourseId, SelectedSemesterId);
                EnrolmentsList = await _enrolmentService.GetForCourseInstance(SelectedCourseId, SelectedSemesterId);

                if (AddNewStudent)
                    StudentsList = await _studentService.GetAll();
            }

            AnyExistingExams = ExamsList != null && ExamsList.Any();

            if (!Context.IsPostBack)
            {
                if (!AnyExistingExams)
                    IsExistingExams = false;
            }

            await base.Load();
        }

        public override async Task PreRender()
        {
            if (IsExistingExams && AnyExistingExams && !StudentOralExam.ExamId.HasValue)
            {
                StudentOralExam.ExamId = ExamsList?.FirstOrDefault()?.Id;
            }

            await CheckExamDate();
            
            await base.PreRender();
        }

        public async Task CheckExamDate()
        {
            ExamDateTimeNotSet = StudentOralExam.ExamId.HasValue ? !(await _examService.HasDate(StudentOralExam.ExamId.Value)) : false;
        }

        public async Task CheckCourseInstanceExist()
        {
            CourseInstanceExist = await _courseService.CourseInstanceExists(SelectedCourseId, SelectedSemesterId);
            ExamDateTimeNotSet = false;
            StudentOralExam.ExamId = null;
            CancelAddStudent();
        }

        public async Task NewStudent()
        {
            StudentsList = await _studentService.GetAll();
            AddNewStudent = true;

            NewEnrolment = new EnrolmentBaseCreate
            {
                CourseInstance = new CourseInstanceBase
                {
                    CourseId = SelectedCourseId,
                    SemesterId = SelectedSemesterId
                }
            };
            SelectedStudent = null;
            SearchStudent = string.Empty;
        }

        public async Task AddStudent()
        {
            ValidationErrors = null;
            try
            {
                if (!CourseInstanceExist)
                {
                    await _courseService.CreateInstance(SelectedCourseId, SelectedSemesterId);
                    CourseInstanceExist = true;
                }

                if (SelectedStudent != null)
                    NewEnrolment.StudentId = SelectedStudent.Id;

                await _enrolmentService.CreateBase(NewEnrolment);

                EnrolmentsList = await _enrolmentService.GetForCourseInstance(SelectedCourseId, SelectedSemesterId);
                CancelAddStudent();
                SearchEnrolment = string.Empty;
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

        public void CancelAddStudent()
        {
            StudentOralExam.EnrolmentId = 0;
            AddNewStudent = false;
            NewEnrolment = null;
            SelectedStudent = null;
            SearchStudent = string.Empty;
        }

        public async Task StartExam()
        {
            ValidationErrors = null;

            StudentOralExam.CourseId = SelectedCourseId;
            StudentOralExam.SemesterId = SelectedSemesterId;
            
            if (IsExistingExams && ExamDateTimeNotSet)
            {
                if (!ExamDate.HasValue)
                {
                    ValidationErrors = new List<string> { "Potrebno je odabrati datum ispita! " };
                    return;
                }

                StudentOralExam.ExistingExamDateTime = new OralExamDateTime
                {
                    Date = ExamDate.Value,
                    Time = ExamTime
                };
            }

            if (!IsExistingExams)
            {
                if (!ExamDate.HasValue)
                {
                    ValidationErrors = new List<string> { "Potrebno je odabrati datum ispita! " };
                    return;
                }

                StudentOralExam.ExistingExamDateTime = null;
                StudentOralExam.Exam = new OralExamCreateExam
                {
                    Date = ExamDate.Value,
                    Time = ExamTime,
                    CourseInstance = new CourseInstanceBase
                    {
                        CourseId = SelectedCourseId,
                        SemesterId = SelectedSemesterId
                    }
                };
            }

            if (IsExistingExams && StudentOralExam.ExamId.HasValue)
            {
                ExistingStudentOralExam = await _studentExamService.GetStudentOralExam(StudentOralExam.ExamId.Value, StudentOralExam.EnrolmentId);
            }
            
            if (ExistingStudentOralExam != null)
            {
                Context.ResourceManager.AddStartupScript("$('div[data-id=confirm]').modal('show');");
            }
            else
            {
                await CreateExamAndStart();
            }
        }

        public void ContinueOnExam(Guid id)
        {
            Context.RedirectToRoute("OralExamUpdate", new { Id = id });
        }

        private async Task CreateExamAndStart()
        {
            ValidationErrors = null;

            try
            {
                var studentOralExamId = await _examService.CreateOralExam(StudentOralExam);
                Context.RedirectToRoute("OralExamUpdate", new { Id = studentOralExamId });
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

        private bool IsFormValid()
        {
            if ((!CourseInstanceExist) ||
                 (IsExistingExams && !StudentOralExam.ExamId.HasValue) ||
                 (!IsExistingExams && StudentOralExam.Exam == null) ||
                 (StudentOralExam.EnrolmentId == 0) ||
                 (IsExistingExams && ExamDateTimeNotSet && !ExamDate.HasValue) ||
                 (!IsExistingExams && !ExamDate.HasValue)
                )
            {
                return false;
            }

            return true;
        }
    }
}
