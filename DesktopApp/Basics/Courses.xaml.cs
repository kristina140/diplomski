using CoreApp.IServices;
using DesktopApp.ViewModels.Basics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopApp.Basics
{
    /// <summary>
    /// Interaction logic for Courses.xaml
    /// </summary>
    public partial class Courses : Page
    {
        private readonly CoursesViewModel viewModel;

        public Courses(ICourseService courseService)
        {
            viewModel = new CoursesViewModel(courseService);
            //DataContext = viewModel;

            InitializeComponent();

            this.Loaded += CoursesTable_Load;
        }


        private async void CoursesTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            //if (viewModel.Courses == null || viewModel.Courses.Count == 0)
            //{
            //    NoCoursesMssg.Visibility = Visibility.Visible;
            //    CoursesTable.Visibility = Visibility.Hidden;
            //}
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
        }

        private void CoursesTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
