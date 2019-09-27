using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopApp.ViewModels.Student
{
    public class StudentCardViewModel : ViewModelBase
    {
        private readonly IStudentService _studentService;

        public StudentCardViewModel(IStudentService studentService)
        {
            _studentService = studentService;

            _getStudentCard = new DelegateCommand(OnGetStudentCard, CanGetStudentCard);
        }

        private StudentCard _studentCard;
        public StudentCard StudentCard
        {
            get => _studentCard;
            set => SetProperty(ref _studentCard, value);
        }

        private bool _showCard;
        public bool ShowCard
        {
            get => _showCard;
            set => SetProperty(ref _showCard, value);
        }

        public List<StudentBase> Students { get; set; }

        private StudentBase _selectedStudent;
        public StudentBase SelectedStudent
        {
            get => _selectedStudent;
            set => SetProperty(ref _selectedStudent, value);
        }

        public async Task Load()
        {
            //StudentCard = await _studentService.GetStudentCard(StudentId);
            Students = await _studentService.GetAll();

            if (Students.Count > 0)
            {
                SelectedStudent = Students.First();
            }
        }

        private readonly DelegateCommand _getStudentCard;
        public ICommand GetStudentCard => _getStudentCard;

        protected void OnGetStudentCard(object commandParameter)
        {
            if (SelectedStudent != null)
            {
                ShowCard = true;
                StudentCard = _studentService.GetStudentCardSync(SelectedStudent.Id);
            }
        }
        protected bool CanGetStudentCard(object commandParameter)
        {
            return true;
        }
    }
}
