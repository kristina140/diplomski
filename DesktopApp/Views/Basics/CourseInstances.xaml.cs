using CoreApp.IServices;
using DesktopApp.ViewModels.Basics;
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

namespace DesktopApp.Views.Basics
{
    /// <summary>
    /// Interaction logic for CourseInstances.xaml
    /// </summary>
    public partial class CourseInstances : Page
    {
        private readonly CourseInstancesViewModel viewModel;

        public CourseInstances(ICourseService courseService, ISemesterService semesterService)
        {
            viewModel = new CourseInstancesViewModel(courseService, semesterService);

            InitializeComponent();

            this.Loaded += CourseInstancesTable_Load;
        }

        private async void CourseInstancesTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.rootGrid.Visibility = Visibility.Visible;
        }
    }
}
