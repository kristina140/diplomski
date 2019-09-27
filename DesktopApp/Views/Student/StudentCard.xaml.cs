using CoreApp.IServices;
using DesktopApp.ViewModels.Student;
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

namespace DesktopApp.Views.Student
{
    /// <summary>
    /// Interaction logic for StudentCard.xaml
    /// </summary>
    public partial class StudentCard : Page
    {
        private readonly StudentCardViewModel viewModel;
        
        public StudentCard(IStudentService studentService)
        {
            viewModel = new StudentCardViewModel(studentService);

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
