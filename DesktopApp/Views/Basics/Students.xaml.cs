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
    /// Interaction logic for Students.xaml
    /// </summary>
    public partial class Students : Page
    {
        private readonly StudentsViewModel viewModel;

        public Students(IStudentService studentService)
        {
            viewModel = new StudentsViewModel(studentService);

            InitializeComponent();

            this.Loaded += StudentsTable_Load;
        }

        private async void StudentsTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.rootGrid.Visibility = Visibility.Visible;
        }

    }
}
