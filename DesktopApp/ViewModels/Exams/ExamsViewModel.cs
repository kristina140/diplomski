using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Commands;
using DesktopApp.Models;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.ViewModels.Exams
{
    public class ExamsViewModel : UpdateableViewModelBase<ExamUpdateableModel, ExamCreate>
    {
        private readonly IExamService _examService;
        private readonly IStudentExamService _studentExamService;
        private readonly ICourseService _courseService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly ISemesterService _semesterService;
        private readonly IExportService _exportService;
        private readonly IDownloadFileService _downloadFileService;

        public ExamsViewModel(
             IExamService examService,
            ICourseService courseService,
            IEnrolmentService enrolmentService,
            IStudentExamService studentExamService,
            ISemesterService semesterService,
            IExportService exportService,
            IDownloadFileService downloadFileService)
        {
            _examService = examService;
            _courseService = courseService;
            _enrolmentService = enrolmentService;
            _studentExamService = studentExamService;
            _semesterService = semesterService;
            _exportService = exportService;
            _downloadFileService = downloadFileService;

            _startExportCommand = new DelegateCommand(OnStartExport, CanStartExport);
            _exportCommand = new DelegateCommand(OnExport, CanExport);
            _addStudentsCommand = new DelegateCommand(OnAddStudents, CanAddStudents);
            _addStudentsSaveCommand = new DelegateCommand(OnAddStudentsSave, CanAddStudentsSave);
            _addStudentsDiscardCommand = new DelegateCommand(OnAddStudentsDiscard, CanAddStudentsDiscard);
            _studentsCommand = new DelegateCommand(OnStudents, CanStudents);
            _saveEditStudentsCommand = new DelegateCommand(OnSaveEditStudents, CanSaveEditStudents);
            _deleteStudentsCommand = new DelegateCommand(OnDeleteStudents, CanDeleteStudents);
            _saveAllStudentsCommand = new DelegateCommand(OnSaveAllStudents, CanSaveAllStudents);
        }

        public List<ExamUpdateList> Exams { get; set; }
        public List<GradeList> GradesList { get; set; }


        #region add new exam
        public List<ExamTypeList> ExamTypesList { get; set; }
        public List<CourseInstanceList> CourseInstancesList { get; set; }

        private ObservableCollection<Selectable<EnrolmentList>> _enrolmentsList;
        public ObservableCollection<Selectable<EnrolmentList>> EnrolmentsList
        {
            get => _enrolmentsList;
            set => SetProperty(ref _enrolmentsList, value);
        }


        private CourseInstanceBase _selectedCourseInstance;
        public CourseInstanceBase SelectedCourseInstance
        {
            get => _selectedCourseInstance;
            set
            {
                SetProperty(ref _selectedCourseInstance, value);
                NewItem.CourseInstance = value;
                GetEnrolments(value);
            }
        }


        public async void GetEnrolments(CourseInstanceBase courseInstance)
        {
            var eList = await _enrolmentService.GetForCourseInstance(courseInstance);
            var seList = Mapper<EnrolmentList>.MapSelectable(eList);
            EnrolmentsList = new ObservableCollection<Selectable<EnrolmentList>>(seList);
        }
        #endregion

        #region students -> student exams
        private bool _inStudentsMode = false;
        public bool InStudentsMode
        {
            get => _inStudentsMode;
            set => SetProperty(ref _inStudentsMode, value);
        }

        private ObservableCollection<StudentExamUpdateList> _studentExams;
        public ObservableCollection<StudentExamUpdateList> StudentExams
        {
            get => _studentExams;
            set => SetProperty(ref _studentExams, value);
        }


        private StudentExamUpdateList _selectedStudentExam;
        public StudentExamUpdateList SelectedStudentExam
        {
            get => _selectedStudentExam;
            set => SetProperty(ref _selectedStudentExam, value);
        }
        private int? _selectedStudentExamIndex;
        public int? SelectedStudentExamIndex
        {
            get => _selectedStudentExamIndex;
            set => SetProperty(ref _selectedStudentExamIndex, value);
        }


        private readonly DelegateCommand _studentsCommand;
        public ICommand StudentsCommand => _studentsCommand;

        private readonly DelegateCommand _saveEditStudentsCommand;
        public ICommand SaveEditStudentsCommand => _saveEditStudentsCommand;

        private readonly DelegateCommand _deleteStudentsCommand;
        public ICommand DeleteStudentsCommand => _deleteStudentsCommand;

        private readonly DelegateCommand _saveAllStudentsCommand;
        public ICommand SaveAllStudentsCommand => _saveAllStudentsCommand;

        protected async void OnStudents(object commandParameter = null)
        {
            InStudentsMode = true;

            var studentExams = await _studentExamService.GetStudentExamsForExam(SelectedItem.Exam.Id);
            StudentExams = new ObservableCollection<StudentExamUpdateList>(studentExams);

            OnAddStudentsDiscard();
        }
        protected bool CanStudents(object commandParameter)
        {
            return true;
        }

        protected async void OnSaveEditStudents(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedStudentExam != null)
            {
                try
                {
                    await _studentExamService.Update(SelectedStudentExam.StudentExam);
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
        }
        protected bool CanSaveEditStudents(object commandParameter)
        {
            return true;
        }

        protected async void OnDeleteStudents(object commandParameter)
        {
            ValidationErrors = null;
            if(SelectedStudentExam != null)
            {
                try
                {
                    await _studentExamService.Delete(SelectedStudentExam.StudentExam.EnrolmentId, SelectedStudentExam.StudentExam.ExamId);

                    StudentExams.Remove(SelectedStudentExam);
                }
                catch (ValidationException ve)
                {
                    ValidationErrors = ve.Errors.ToList();
                }
            }
        }
        protected bool CanDeleteStudents(object commandParameter)
        {
            return true;
        }

        protected async void OnSaveAllStudents(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _studentExamService.Update(StudentExams.Select(_ => _.StudentExam).ToList());
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
        protected bool CanSaveAllStudents(object commandParameter)
        {
            return true;
        }
        #endregion

        #region add students to exam
        private bool _inAddStudentsMode = false;
        public bool InAddStudentsMode
        {
            get => _inAddStudentsMode;
            set => SetProperty(ref _inAddStudentsMode, value);
        }


        private ObservableCollection<Selectable<EnrolmentList>> _studentsList;
        public ObservableCollection<Selectable<EnrolmentList>> StudentsList
        {
            get => _studentsList;
            set => SetProperty(ref _studentsList, value);
        }


        private readonly DelegateCommand _addStudentsCommand;
        public ICommand AddStudentsCommand => _addStudentsCommand;

        private readonly DelegateCommand _addStudentsSaveCommand;
        public ICommand AddStudentsSaveCommand => _addStudentsSaveCommand;

        private readonly DelegateCommand _addStudentsDiscardCommand;
        public ICommand AddStudentsDiscardCommand => _addStudentsDiscardCommand;


        protected async void OnAddStudents(object commandParameter)
        {
            InAddStudentsMode = true;

            var enrolments = await _enrolmentService.GetAvailableForCourseInstance(new CourseInstanceBase
            {
                CourseId = SelectedItem.Exam.Course.Id,
                SemesterId = SelectedItem.Exam.Semester.Id
            }, SelectedItem.Exam.Id);

            var selectables = Mapper<EnrolmentList>.MapSelectable(enrolments);

            StudentsList = new ObservableCollection<Selectable<EnrolmentList>>(selectables);
        }
        protected bool CanAddStudents(object commandParameter)
        {
            return true;
        }

        protected async void OnAddStudentsSave(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                var seList = StudentsList?.Where(_ => _.IsSelected)?.Select(_ => new StudentExamCreate
                {
                    ExamId = SelectedItem.Exam.Id,
                    EnrolmentId = _.Item.Id
                }).ToList();

                await _studentExamService.Create(seList);

                OnAddStudentsDiscard();
                OnStudents();
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
        protected bool CanAddStudentsSave(object commandParameter)
        {
            return true;
        }

        protected void OnAddStudentsDiscard(object commandParameter = null)
        {
            InAddStudentsMode = false;
            StudentsList = null;
        }
        protected bool CanAddStudentsDiscard(object commandParameter)
        {
            return true;
        }
        #endregion

        #region export 
        private bool _inExportMode = false;
        public bool InExportMode
        {
            get => _inExportMode;
            set => SetProperty(ref _inExportMode, value);
        }

        public List<CourseList> CoursesList { get; set; }
        public List<SemesterList> SemestersList { get; set; }

        private int _selectedCourse;
        public int SelectedCourse
        {
            get => _selectedCourse;
            set => SetProperty(ref _selectedCourse, value);
        }
        private int _selectedSemester;
        public int SelectedSemester
        {
            get => _selectedSemester;
            set => SetProperty(ref _selectedSemester, value);
        }


        private readonly DelegateCommand _startExportCommand;
        public ICommand StartExportCommand => _startExportCommand;

        private readonly DelegateCommand _exportCommand;
        public ICommand ExportCommand => _exportCommand;


        protected void OnStartExport(object commandParameter)
        {
            InExportMode = true;

            var selectedC = CoursesList.FirstOrDefault();
            if (selectedC != null)
                SelectedCourse = selectedC.Id;

            var selectedS = SemestersList.FirstOrDefault();
            if (selectedS != null)
                SelectedSemester = selectedS.Id;
        }
        protected bool CanStartExport(object commandParameter)
        {
            return true;
        }

        protected void OnExport(object commandParameter)
        {
            ValidationErrors = null;
            var data = _studentExamService.GetStudentExamsExportSync(SelectedCourse, SelectedSemester);

            if (data == null)
            {
                ValidationErrors = new List<string> { "Nema studentskih ispita za odabrani predmet i semestar." };
            }
            else
            {
                var File = _exportService.ExportStudentExams(data, "ispiti.xlsx");
                _downloadFileService.Download(File);
            }
        }
        protected bool CanExport(object commandParameter)
        {
            return true;
        }
        #endregion
       

        public async Task Load()
        {
            Exams = await _examService.GetUpdateable();
            ItemsObservable = new ObservableCollection<ExamUpdateableModel>(
                Exams.Select(_ => Mapper.MapExamUpdateableModel(_)));

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

            CoursesList = await _courseService.GetList();
            SemestersList = await _semesterService.GetList();
        }


        protected override async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _examService.ForceDelete(SelectedItem.Exam.Id);

                    Exams.RemoveAt(SelectedItemIndex.Value);
                    ItemsObservable.Remove(SelectedItem);
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
        }

        protected override void OnDiscardEdit(object commandParameter)
        {
            if (SelectedItemIndex.HasValue)
            {
                var oldValues = Exams.FirstOrDefault(_ => SelectedItem.Exam.Id == _.Id);

                ItemsObservable[SelectedItemIndex.Value] = Mapper.MapExamUpdateableModel(oldValues);
            }
        }

        protected override void OnDiscardAdd(object commandParameter = null)
        {
            SelectedCourseInstance = null;
            EnrolmentsList = null;

            base.OnDiscardAdd(commandParameter);
        }

        protected override async void OnSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                var selectedStudents = EnrolmentsList?.Where(_ => _.IsSelected)?.Select(_ => _.Item.Id);
                if (selectedStudents != null && selectedStudents.Any())
                {
                    NewItem.EnrolmentIds = selectedStudents.ToList();
                }

                await _examService.Create(NewItem);

                await Load();

                OnDiscardAdd();
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

        protected override async void OnSaveEdit(object commandParameter)
        {
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                ValidationErrors = null;
                try
                {
                    var newValues = await _examService.Update(SelectedItem.Exam.Id, SelectedItem.Exam.Exam);
                    
                    Exams[SelectedItemIndex.Value].Exam = newValues;

                    ItemsObservable[SelectedItemIndex.Value].Exam.Exam = new ExamUpdate { 
                        Id = newValues.Id, 
                        Date = newValues.Date, 
                        Time = newValues.Time
                    };
                    ItemsObservable[SelectedItemIndex.Value].InEditMode = false;

                    ValidationErrors = null;
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
        }

    }
}
