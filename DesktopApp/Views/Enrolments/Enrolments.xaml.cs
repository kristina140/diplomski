using CoreApp.IServices;
using DesktopApp.ViewModels.Enrolments;
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

namespace DesktopApp.Views.Enrolments
{
    /// <summary>
    /// Interaction logic for Enrolments.xaml
    /// </summary>
    public partial class Enrolments : Page
    {
        private readonly EnrolmentsViewModel viewModel;

        public Enrolments(
            IEnrolmentService enrolmentService, 
            ICourseService courseService,
            IStudentService studentService)
        {
            viewModel = new EnrolmentsViewModel(enrolmentService, courseService, studentService);

            InitializeComponent();

            this.Loaded += EnrolmentsTable_Load;
        }

        private async void EnrolmentsTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.rootGrid.Visibility = Visibility.Visible;
        }
    }
}
