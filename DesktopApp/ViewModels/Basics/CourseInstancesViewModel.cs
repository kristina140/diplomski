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

namespace DesktopApp.ViewModels.Basics
{
    public class CourseInstancesViewModel : AddOnlyViewModelBase<CourseInstanceListModel, CourseInstanceCreate>
    {
        private readonly ICourseService _courseService;
        private readonly ISemesterService _semesterService;

        public CourseInstancesViewModel(
            ICourseService courseService, 
            ISemesterService semesterService)
        {
            _courseService = courseService;
            _semesterService = semesterService;

            _courseStartAddCommand = new DelegateCommand(OnCourseStartAdd, CanCourseStartAdd);
            _semesterStartAddCommand = new DelegateCommand(OnSemesterStartAdd, CanSemesterStartAdd);

            _courseSaveAddCommand = new DelegateCommand(OnCourseSaveAdd, CanCourseSaveAdd);
            _semesterSaveAddCommand = new DelegateCommand(OnSemesterSaveAdd, CanSemesterSaveAdd);

            _courseDiscardAddCommand = new DelegateCommand(OnCourseDiscardAdd, CanCourseDiscardAdd);
            _semesterDiscardAddCommand = new DelegateCommand(OnSemesterDiscardAdd, CanSemesterDiscardAdd);
        }
        
        public List<CourseInstanceList> CourseInstances { get; set; }

        private ObservableCollection<CourseList> _coursesList;
        public ObservableCollection<CourseList> CoursesList
        {
            get => _coursesList;
            set => SetProperty(ref _coursesList, value);
        }

        private ObservableCollection<SemesterList> _semestersList;
        public ObservableCollection<SemesterList> SemestersList
        {
            get => _semestersList;
            set => SetProperty(ref _semestersList, value);
        }

        private CourseCreate _newCourse;
        public CourseCreate NewCourse
        {
            get => _newCourse;
            set => SetProperty(ref _newCourse, value);
        }

        private SemesterCreateModel _newSemester;
        public SemesterCreateModel NewSemester
        {
            get => _newSemester;
            set => SetProperty(ref _newSemester, value);
        }

        private bool _inAddSemesterMode = false;
        public bool InAddSemesterMode
        {
            get => _inAddSemesterMode;
            set => SetProperty(ref _inAddSemesterMode, value);
        }

        private bool _inAddCourseMode = false;
        public bool InAddCourseMode
        {
            get => _inAddCourseMode;
            set => SetProperty(ref _inAddCourseMode, value);
        }

        public async Task Load()
        {
            CourseInstances = await _courseService.GetInastancesList();

            ItemsObservable = new ObservableCollection<CourseInstanceListModel>(
                CourseInstances.Select(_ => Mapper.MapCourseInstanceListModel(_)));

            await LoadCourses();
            await LoadSemesters();
        }

        private async Task LoadCourses()
        {
            var coursesList = await _courseService.GetList();
            CoursesList = new ObservableCollection<CourseList>(coursesList);
        }

        private async Task LoadSemesters()
        {
            var semestersList = await _semesterService.GetList();
            SemestersList = new ObservableCollection<SemesterList>(semestersList);
        }

        protected override async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _courseService.DeleteInstance(SelectedItem.CourseInstance.Course.Id, SelectedItem.CourseInstance.Semester.Id);

                    CourseInstances.RemoveAt(SelectedItemIndex.Value);
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

        protected override async void OnSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateInstance(NewItem.CourseId, NewItem.SemesterId);

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

        protected override void OnDiscardAdd(object commandParameter = null)
        {
            OnCourseDiscardAdd();
            OnSemesterDiscardAdd();

            base.OnDiscardAdd(commandParameter);
        }


        private readonly DelegateCommand _courseStartAddCommand;
        public ICommand CourseStartAddCommand => _courseStartAddCommand;

        private readonly DelegateCommand _semesterStartAddCommand;
        public ICommand SemesterStartAddCommand => _semesterStartAddCommand;

        private readonly DelegateCommand _courseSaveAddCommand;
        public ICommand CourseSaveAddCommand => _courseSaveAddCommand;

        private readonly DelegateCommand _semesterSaveAddCommand;
        public ICommand SemesterSaveAddCommand => _semesterSaveAddCommand;

        private readonly DelegateCommand _courseDiscardAddCommand;
        public ICommand CourseDiscardAddCommand => _courseDiscardAddCommand;

        private readonly DelegateCommand _semesterDiscardAddCommand;
        public ICommand SemesterDiscardAddCommand => _semesterDiscardAddCommand;

        private void OnCourseStartAdd(object commandParameter)
        {
            InAddCourseMode = true;
            NewCourse = new CourseCreate();
        }
        protected bool CanCourseStartAdd(object commandParameter)
        {
            return true;
        }

        private void OnSemesterStartAdd(object commandParameter)
        {
            InAddSemesterMode = true;
            NewSemester = new SemesterCreateModel() { StartDate = DateTime.Now };
        }
        protected bool CanSemesterStartAdd(object commandParameter)
        {
            return true;
        }

        protected async void OnCourseSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _courseService.CreateBasic(NewCourse);

                await LoadCourses();

                OnCourseDiscardAdd();
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
        protected bool CanCourseSaveAdd(object commandParameter)
        {
            return true;
        }

        protected async void OnSemesterSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _semesterService.Create(Mapper.MapSemesterCreate(NewSemester));

                await LoadSemesters();

                OnSemesterDiscardAdd();
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
        protected bool CanSemesterSaveAdd(object commandParameter)
        {
            return true;
        }

        protected void OnCourseDiscardAdd(object commandParameter = null)
        {
            ValidationErrors = null;
            InAddCourseMode = false;
            NewCourse = null;
        }

        protected bool CanCourseDiscardAdd(object commandParameter)
        {
            return true;
        }

        protected void OnSemesterDiscardAdd(object commandParameter = null)
        {
            ValidationErrors = null;
            InAddSemesterMode = false;
            NewSemester = null;
        }

        protected bool CanSemesterDiscardAdd(object commandParameter)
        {
            return true;
        }
    }
}
