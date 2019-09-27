using CoreApp.IServices;
using DesktopApp.ViewModels.Exams;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Views.Exams
{
    /// <summary>
    /// Interaction logic for OralExam.xaml
    /// </summary>
    public partial class OralExam : Page
    {
        private readonly OralExamViewModel viewModel;

        public OralExam(
             IExamService examService,
            ICourseService courseService,
            ISemesterService semesterService,
            IEnrolmentService enrolmentService,
            IStudentService studentService,
            IStudentExamService studentExamService)
        {
            viewModel = new OralExamViewModel(examService, courseService, semesterService, enrolmentService, studentService, studentExamService);

            InitializeComponent();

            this.Loaded += Data_Load;
        }

        private async void Data_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.ContentGrid.Visibility = Visibility.Visible;
        }
    }
}
