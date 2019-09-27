using CoreApp.IServices;
using DesktopApp.Utility;
using DesktopApp.Views.Basics;
using DesktopApp.Views.Enrolments;
using DesktopApp.Views.Exams;
using DesktopApp.Views.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ICourseService _courseService;
        private readonly IStudentService _studentService;
        private readonly ISemesterService _semesterService;
        private readonly IStudentExamService _studentExamService;
        private readonly IEnrolmentService _enrolmentService;
        private readonly IExamService _examService;

        private readonly IExportService _exportService;
        private readonly IDownloadFileService _downloadFileService;

        public MainWindow(
            ICourseService courseService,
            IStudentService studentService,
            ISemesterService semesterService,
            IStudentExamService studentExamService,
            IEnrolmentService enrolmentService,
            IExamService examService,
            IExportService exportService,
            IDownloadFileService downloadFileService)
        {
            _courseService = courseService;
            _studentService = studentService;
            _semesterService = semesterService;
            _studentExamService = studentExamService;
            _enrolmentService = enrolmentService;
            _examService = examService;

            _exportService = exportService;
            _downloadFileService = downloadFileService;


            InitializeComponent();
        }

        private void NavigateHome_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Navigate(new System.Uri("Home.xaml", UriKind.RelativeOrAbsolute));
        }

        private void NavigateCourses_Click(object sender, RoutedEventArgs e)
        {
            var page = new Courses(_courseService);
            MainContent.Navigate(page);
        }

        private void NavigateSemesters_Click(object sender, RoutedEventArgs e)
        {
            var page = new Semesters(_semesterService);
            MainContent.Navigate(page);
        }

        private void NavigateStudents_Click(object sender, RoutedEventArgs e)
        {
            var page = new Students(_studentService);
            MainContent.Navigate(page);
        }

        private void NavigateCourseInstances_Click(object sender, RoutedEventArgs e)
        {
            var page = new CourseInstances(_courseService, _semesterService);
            MainContent.Navigate(page);
        }

        private void NavigateEnrolments_Click(object sender, RoutedEventArgs e)
        {
            var page = new Enrolments(_enrolmentService, _courseService, _studentService);
            MainContent.Navigate(page);
        }

        private void NavigateExams_Click(object sender, RoutedEventArgs e)
        {
            var page = new Exams(_examService, _courseService, _enrolmentService, _studentExamService, _semesterService, _exportService, _downloadFileService);
            MainContent.Navigate(page);
        }

        private void NavigateStudentCard_Click(object sender, RoutedEventArgs e)
        {
            var page = new StudentCard(_studentService);
            MainContent.Navigate(page);
        }

        private void NavigateOralExam_Click(object sender, RoutedEventArgs e)
        {
            var page = new OralExam(_examService, _courseService, _semesterService, _enrolmentService, _studentService, _studentExamService);
            MainContent.Navigate(page);
        }

        private void NavigateOralExamUpdate_Click(object sender, RoutedEventArgs e)
        {
            var page = new OralExamUpdate();
            MainContent.Navigate(page);
        }
    }
}
