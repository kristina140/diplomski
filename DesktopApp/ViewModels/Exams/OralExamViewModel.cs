using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Commands;
using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.ViewModels.Exams
{
    public class OralExamViewModel : ViewModelBase
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

            _startExamCommand = new DelegateCommand(OnStartExam, CanStartExam);
            _selectExistingExam = new DelegateCommand(OnSelectExistingExam, CanSelectExistingExam);
            _selectNewExam = new DelegateCommand(OnSelectNewExam, CanSelectNewExam);
        }

        private OralExamCreate _studentOralExam = new OralExamCreate();
        public OralExamCreate StudentOralExam
        {
            get => _studentOralExam;
            set => SetProperty(ref _studentOralExam, value);
        }

        public List<CourseList> CoursesList { get; set; }
        private CourseList _selectedCourse;
        public CourseList SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                SetProperty(ref _selectedCourse, value);

                TryLoadAdditionalData();
            }
        }

        public List<SemesterList> SemestersList { get; set; }
        private SemesterList _selectedSemester;
        public SemesterList SelectedSemester
        {
            get => _selectedSemester;
            set
            {
                SetProperty(ref _selectedSemester, value);

                TryLoadAdditionalData();
            }
        }

        private bool _courseInstanceExist;
        public bool CourseInstanceExist
        {
            get => _courseInstanceExist;
            set
            {
                SetProperty(ref _courseInstanceExist, value);
                validateForm();
            }
        }


        private ObservableCollection<ExamList> _examList;
        public ObservableCollection<ExamList> ExamsList
        {
            get => _examList;
            set => SetProperty(ref _examList, value);
        }
        private ExamList _selectedExam;
        public ExamList SelectedExam
        {
            get => _selectedExam;
            set
            {
                SetProperty(ref _selectedExam, value);

                ExamHasDate = value != null ? value.Date.HasValue : false;

                if (ExamHasDate)
                {
                    StudentOralExam.ExistingExamDateTime = new OralExamDateTime
                    {
                        Date = value.Date.Value.Date,
                        Time = value.Date.Value.Date
                    };
                }
               
            }
        }

        private bool _examHasDate;
        public bool ExamHasDate
        {
            get => _examHasDate;
            set => SetProperty(ref _examHasDate, value);
        }


        private ObservableCollection<EnrolmentList> _enrolmentsList;
        public ObservableCollection<EnrolmentList> EnrolmentsList
        {
            get => _enrolmentsList;
            set => SetProperty(ref _enrolmentsList, value);
        }
        private EnrolmentList _selectedEnrolment;
        public EnrolmentList SelectedEnrolment
        {
            get => _selectedEnrolment;
            set
            {
                SetProperty(ref _selectedEnrolment, value);
                validateForm();
            }
        }


        private bool _isExistingExam = false;
        public bool IsExistingExam
        {
            get => _isExistingExam;
            set 
            { 
                SetProperty(ref _isExistingExam, value);
                validateForm();
            }
        }

        private bool _isValidForm;
        public bool IsValidForm
        {
            get => _isValidForm;
            set => SetProperty(ref _isValidForm, value);
        }
        private void validateForm()
        {
            IsValidForm = CourseInstanceExist &&
                 ((IsExistingExam && SelectedExam != null) || (!IsExistingExam)) &&
                 SelectedEnrolment != null;
        }

        public async Task Load()
        {
            CoursesList = await _courseService.GetList();
            SemestersList = await _semesterService.GetList();

            if (CoursesList.Any())
            {
                SelectedCourse = CoursesList.First();
            }

            if (SemestersList.Any())
            {
                SelectedSemester = SemestersList.First();
            }

            GradesList = Enum.GetValues(typeof(Grade)).OfType<Grade>().ToList().Select(_ => new GradeList
            {
                Grade = _,
                Description = _.GetEnumDescription()
            }).ToList();
        }

        private async void CourseInstanceExists()
        {
            CourseInstanceExist = SelectedCourse == null || SelectedSemester == null ? false :
                await _courseService.CourseInstanceExists(SelectedCourse.Id, SelectedSemester.Id);
        }

        private async void TryLoadAdditionalData()
        {
            CourseInstanceExists();
            if (CourseInstanceExist)
            {
                var eList = await _examService.GetOralExamsListForCourseInstance(SelectedCourse.Id, SelectedSemester.Id);
                ExamsList = new ObservableCollection<ExamList>(eList);

                var enList = await _enrolmentService.GetForCourseInstance(SelectedCourse.Id, SelectedSemester.Id);
                EnrolmentsList = new ObservableCollection<EnrolmentList>(enList);

                IsExistingExam = ExamsList.Any();
            }
        }


        private readonly DelegateCommand _startExamCommand;
        public ICommand StartExamCommand => _startExamCommand;

        public ExistingStudentOralExam ExistingStudentOralExam { get; set; } = null;
        protected async void OnStartExam(object commandParameter = null)
        {
            if (SelectedEnrolment == null || SelectedCourse == null || SelectedSemester == null)
            {
                ValidationErrors = new List<string> { "Potrebno je odabrati predmet, semestar i studenta! " };
                return;
            }

            ValidationErrors = null;

            StudentOralExam.CourseId = SelectedCourse.Id;
            StudentOralExam.SemesterId = SelectedSemester.Id;
            StudentOralExam.EnrolmentId = SelectedEnrolment.Id;

            if (IsExistingExam )
            {
                StudentOralExam.ExamId = SelectedExam.Id;
            }

            if (!IsExistingExam)
            {
                StudentOralExam.ExistingExamDateTime = null;
                StudentOralExam.Exam = new OralExamCreateExam
                {
                    CourseInstance = new CourseInstanceBase
                    {
                        CourseId = SelectedCourse.Id,
                        SemesterId = SelectedSemester.Id
                    }
                };
            }

            if (IsExistingExam && StudentOralExam.ExamId.HasValue)
            {
                ExistingStudentOralExam = await _studentExamService.GetStudentOralExam(StudentOralExam.ExamId.Value, StudentOralExam.EnrolmentId);
            }

            if (ExistingStudentOralExam == null)
            {
                ValidationErrors = null;

                try
                {
                    var studentOralExamId = await _examService.CreateOralExam(StudentOralExam);
                    ExistingStudentOralExam = await _studentExamService.GetStudentOralExam(StudentOralExam.ExamId.Value, StudentOralExam.EnrolmentId);
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

            startExam();
        }
        protected bool CanStartExam(object commandParameter = null)
        {
            return true;
        }


        private readonly DelegateCommand _selectExistingExam;
        public ICommand SelectExistingExam => _selectExistingExam;

        protected void OnSelectExistingExam(object commandParameter = null)
        {
            IsExistingExam = true;
        }
        protected bool CanSelectExistingExam(object commandParameter = null)
        {
            return true;
        }

        private readonly DelegateCommand _selectNewExam;
        public ICommand SelectNewExam => _selectNewExam;

        protected void OnSelectNewExam(object commandParameter = null)
        {
            IsExistingExam = false;
        }
        protected bool CanSelectNewExam(object commandParameter = null)
        {
            return true;
        }


        public List<GradeList> GradesList { get; set; }


        private bool _examStarted = false;
        public bool ExamStarted
        {
            get => _examStarted;
            set => SetProperty(ref _examStarted, value);
        }

        private List<StudentExamList> _studentExams;
        public List<StudentExamList> StudentExams
        {
            get => _studentExams;
            set => SetProperty(ref _studentExams, value);
        }

        private OralExamList _oralExamListData;
        public OralExamList OralExamListData
        {
            get => _oralExamListData;
            set => SetProperty(ref _oralExamListData, value);
        }

        private OralExamUpdate _oralExamUpdate = new OralExamUpdate();
        public OralExamUpdate OralExamData
        {
            get => _oralExamUpdate;
            set => SetProperty(ref _oralExamUpdate, value);
        }
        private async void startExam()
        {
            ExamStarted = true;

            OralExamListData = await _studentExamService.GetOralExam(ExistingStudentOralExam.Id);

            StudentExams = await _studentExamService.GetStudentExamList(OralExamListData.Enrolment.Student.Id, ExistingStudentOralExam.Id, OralExamListData.CourseId);

          
        }

        private readonly DelegateCommand _saveExamCommand;
        public ICommand SaveExamCommand => _saveExamCommand;

        protected async void OnSaveOralExam(object commandParameter = null)
        {
            OralExamData.Id = ExistingStudentOralExam.Id;
            OralExamData.ExamId = OralExamListData.Exam.Id;
            OralExamData.EnrolmentId = OralExamListData.Enrolment.Id;

            ValidationErrors = null;

            try
            {
                await _studentExamService.UpdateOralExam(OralExamData);
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
        protected bool CanSaveOralExam(object commandParameter = null)
        {
            return true;
        }
    }
}
