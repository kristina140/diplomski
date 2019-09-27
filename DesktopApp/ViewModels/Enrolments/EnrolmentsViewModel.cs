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

namespace DesktopApp.ViewModels.Enrolments
{
    public class EnrolmentsViewModel : ViewModelBase
    {
        private readonly IEnrolmentService _enrolmentService;
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;

        public EnrolmentsViewModel(
            IEnrolmentService enrolmentService,
            ICourseService courseService,
            IStudentService studentService)
        {
            _enrolmentService = enrolmentService;
            _courseService = courseService;
            _studentService = studentService;

            _editItemCommand = new DelegateCommand(OnEditItem, CanEditItem);
            _discardEditCommand = new DelegateCommand(OnDiscardEdit, CanDiscardEdit);
            _saveEditCommand = new DelegateCommand(OnSaveEdit, CanSaveEdit);
            _deleteItemCommand = new DelegateCommand(OnDeleteItem, CanDeleteItem);
            _startAddCommand = new DelegateCommand(OnStartAdd, CanStartAdd);
            _discardAddCommand = new DelegateCommand(OnDiscardAdd, CanDiscardAdd);
            _saveSafeAddCommand = new DelegateCommand(OnSafeSaveAdd, CanSafeSaveAdd);
            _saveNewAddCommand = new DelegateCommand(OnNewSaveAdd, CanNewSaveAdd);

            _pendingAddCommand = new DelegateCommand(OnPendingAdd, CanPendingAdd);
            _pendingDeleteCommand = new DelegateCommand(OnPendingDelete, CanPendingDelete);

            _toggleDetailsCommand = new DelegateCommand(OnToggleDetails, CanToggleDetails);
        }

        public List<EnrolmentUpdateList> Enrolments { get; set; }

        private ObservableCollection<EnrolmentUpdateableModel> _itemsObservable;
        public ObservableCollection<EnrolmentUpdateableModel> ItemsObservable
        {
            get => _itemsObservable;
            set => SetProperty(ref _itemsObservable, value);
        }

        public List<GradeList> GradesList { get; set; }

        private List<StudentBase> students;
        private ObservableCollection<Selectable<StudentBase>> _studentsList;
        public ObservableCollection<Selectable<StudentBase>> StudentsList 
        {
            get => _studentsList;
            set => SetProperty(ref _studentsList, value); 
        }

        private List<CourseInstanceList> courseInstances;
        private ObservableCollection<Selectable<CourseInstanceList>> _courseInstances;
        public ObservableCollection<Selectable<CourseInstanceList>> CourseInstancesList
        {
            get => _courseInstances;
            set => SetProperty(ref _courseInstances, value);
        }

        private ObservableCollection<EnrolmentCreateModel> _newItemsObservable;
        public ObservableCollection<EnrolmentCreateModel> NewItemsObservable
        {
            get => _newItemsObservable;
            set => SetProperty(ref _newItemsObservable, value);
        }


        #region properties
        private EnrolmentUpdateableModel _selectedItem;
        public EnrolmentUpdateableModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }
        private int? _selectedItemIndex;
        public int? SelectedItemIndex
        {
            get => _selectedItemIndex;
            set => SetProperty(ref _selectedItemIndex, value);
        }

        private bool _inAddMode = false;
        public bool InAddMode
        {
            get => _inAddMode;
            set => SetProperty(ref _inAddMode, value);
        }

        private bool _inPendingMode = false;
        public bool InPendingMode
        {
            get => _inPendingMode;
            set => SetProperty(ref _inPendingMode, value);
        }

        private bool _toggleDetails = false;
        public bool ToggleDetails
        {
            get => _toggleDetails;
            set => SetProperty(ref _toggleDetails, value);
        }

        private EnrolmentCreateModel _selectedPendingItem;
        public EnrolmentCreateModel SelectedPendingItem
        {
            get => _selectedPendingItem;
            set
            {
                SetProperty(ref _selectedPendingItem, value);
            }
        }
        private int? _selectedPendingItemIndex;
        public int? SelectedPendingItemIndex
        {
            get => _selectedPendingItemIndex;
            set => SetProperty(ref _selectedPendingItemIndex, value);
        }
        #endregion

        #region commands
        private readonly DelegateCommand _editItemCommand;
        public ICommand EditItemCommand => _editItemCommand;

        private readonly DelegateCommand _discardEditCommand;
        public ICommand DiscardEditCommand => _discardEditCommand;

        private readonly DelegateCommand _saveEditCommand;
        public ICommand SaveEditCommand => _saveEditCommand;


        private readonly DelegateCommand _deleteItemCommand;
        public ICommand DeleteItemCommand => _deleteItemCommand;

        private readonly DelegateCommand _startAddCommand;
        public ICommand StartAddCommand => _startAddCommand;

        private readonly DelegateCommand _discardAddCommand;
        public ICommand DiscardAddCommand => _discardAddCommand;

        private readonly DelegateCommand _saveSafeAddCommand;
        public ICommand SaveSafeAddCommand => _saveSafeAddCommand;

        private readonly DelegateCommand _saveNewAddCommand;
        public ICommand SaveNewAddCommand => _saveNewAddCommand;

        private readonly DelegateCommand _toggleDetailsCommand;
        public ICommand ToggleDetailsCommand => _toggleDetailsCommand;

        private readonly DelegateCommand _pendingAddCommand;
        public ICommand PendingAddCommand => _pendingAddCommand;

        private readonly DelegateCommand _pendingDeleteCommand;
        public ICommand PendingDeleteCommand => _pendingDeleteCommand;
        #endregion

        public async Task Load()
        {
            Enrolments = await _enrolmentService.GetUpdateable();

            ItemsObservable = new ObservableCollection<EnrolmentUpdateableModel>(
                Enrolments.Select(_ => Mapper.MapEnrolmentUpdateableModel(_)));

            GradesList = Enum.GetValues(typeof(Grade)).OfType<Grade>().ToList().Select(_ => new GradeList
            {
                Grade = _,
                Description = _.GetEnumDescription()
            }).ToList();

            students = await _studentService.GetAll();
            StudentsList = new ObservableCollection<Selectable<StudentBase>>(
                Mapper<StudentBase>.MapSelectable(students));

            courseInstances = await _courseService.GetInastancesList();
            CourseInstancesList = new ObservableCollection<Selectable<CourseInstanceList>>(
                Mapper<CourseInstanceList>.MapSelectable(courseInstances));
        }

        #region command implementations
        protected void OnEditItem(object commandParameter)
        {
            if (SelectedItemIndex.HasValue && SelectedItemIndex >= 0 && SelectedItemIndex < ItemsObservable.Count)
            {
                ItemsObservable[SelectedItemIndex.Value].InEditMode = true;
            }
        }
        protected bool CanEditItem(object commandParameter)
        {
            return true;
        }

        protected void OnDiscardEdit(object commandParameter)
        {
            if (SelectedItemIndex.HasValue)
            {
                var oldValues = Enrolments.FirstOrDefault(_ => SelectedItem.Enrolment.Id == _.Id);

                ItemsObservable[SelectedItemIndex.Value] = Mapper.MapEnrolmentUpdateableModel(oldValues);
            }
        }
        protected bool CanDiscardEdit(object commandParameter)
        {
            return true;
        }

        protected async void OnSaveEdit(object commandParameter)
        {
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                ValidationErrors = null;
                try
                {
                    var newValues = await _enrolmentService.Update(SelectedItem.Enrolment.Id, Mapper.MapEnrolmentUpdate(SelectedItem));

                    Enrolments[SelectedItemIndex.Value] = await _enrolmentService.GetUpdateable(newValues.Id);

                    ItemsObservable[SelectedItemIndex.Value].MapEnrolmentUpdateableModel(Enrolments[SelectedItemIndex.Value]);
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
        protected bool CanSaveEdit(object commandParameter)
        {
            return true;
        }

        protected async void OnDeleteItem(object commandParameter)
        {
            ValidationErrors = null;
            if (SelectedItem != null && SelectedItemIndex.HasValue)
            {
                try
                {
                    await _enrolmentService.Delete(SelectedItem.Enrolment.Id);

                    Enrolments.RemoveAt(SelectedItemIndex.Value);
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
        protected bool CanDeleteItem(object commandParameter)
        {
            return true;
        }

        protected void OnStartAdd(object commandParameter)
        {
            InAddMode = true;
            NewItemsObservable = new ObservableCollection<EnrolmentCreateModel>();
        }
        protected bool CanStartAdd(object commandParameter)
        {
            return true;
        }

        protected void OnDiscardAdd(object commandParameter = null)
        {
            ValidationErrors = null;
            InAddMode = false;
            NewItemsObservable = null;
            InPendingMode = false;
        }
        protected bool CanDiscardAdd(object commandParameter)
        {
            return true;
        }

        protected async void OnSafeSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _enrolmentService.CreateSafe(NewItemsObservable.Select(_ => _.Enrolment).ToList());

                OnDiscardAdd();
                await Load();
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
        protected bool CanSafeSaveAdd(object commandParameter)
        {
            return true;
        }

        protected async void OnNewSaveAdd(object commandParameter)
        {
            ValidationErrors = null;
            try
            {
                await _enrolmentService.UpdateAndCreate(NewItemsObservable.Select(_ => _.Enrolment).ToList());

                OnDiscardAdd();
                await Load();
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
        protected bool CanNewSaveAdd(object commandParameter)
        {
            return true;
        }

        protected void OnPendingAdd(object commandParameter)
        {
            InPendingMode = true;
            ValidationErrors = null;

            var selectedStudents = StudentsList.Where(_ => _.IsSelected);
            var selectedCoursInstances = CourseInstancesList.Where(_ => _.IsSelected);
            
            var duplicates = new List<string>();

            foreach (var student in selectedStudents)
            {
                foreach (var courseInstance in selectedCoursInstances)
                {
                    if (NewItemsObservable.Any(_ => _.Student.Id == student.Item.Id &&
                        _.Semester.Id == courseInstance.Item.Semester.Id &&
                        _.Course.Id == courseInstance.Item.Course.Id))
                    {
                        duplicates.Add(string.Format($"{student.Item.UserFriendly} - {courseInstance.Item.UserFriendly}"));
                        continue;
                    }
                    NewItemsObservable.Add(Mapper.MapEnrolmentCreateModel(student, courseInstance));

                }
            }

            if (duplicates.Any())
            {
                ValidationErrors = new List<string>
                {
                    "Duplikati :"
                };

                duplicates.ForEach(_ => ValidationErrors.Add(_));

                ValidationErrors.Add("nisu dodani ponovno.");
            }
        }
        protected bool CanPendingAdd(object commandParameter)
        {
            return true;
        }

        protected void OnPendingDelete(object commandParameter)
        {
            if (SelectedPendingItem != null && SelectedPendingItemIndex.HasValue)
            {
                NewItemsObservable.Remove(SelectedPendingItem);
            }
        }
        protected bool CanPendingDelete(object commandParameter)
        {
            return true;
        }

        protected void OnToggleDetails(object commandParemeter = null)
        {
            ToggleDetails = !ToggleDetails;
        }
        protected bool CanToggleDetails(object commandParameter = null)
        {
            return true;
        }
        #endregion

    }
}
