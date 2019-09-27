using CoreApp.IServices;
using DesktopApp.Utility;
using DesktopApp.ViewModels.Exams;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Exams.xaml
    /// </summary>
    public partial class Exams : Page
    {
        private readonly ExamsViewModel viewModel;

        public Exams(
            IExamService examService,
            ICourseService courseService,
            IEnrolmentService enrolmentService,
            IStudentExamService studentExamService,
            ISemesterService semesterService,
            IExportService exportService,
            IDownloadFileService downloadFileService)
        {
            viewModel = new ExamsViewModel(examService, courseService, enrolmentService, studentExamService, semesterService, exportService, downloadFileService);

            InitializeComponent();

            this.Loaded += ExamsTable_Load;
        }

        private async void ExamsTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.rootGrid.Visibility = Visibility.Visible;
        }

    }
}
