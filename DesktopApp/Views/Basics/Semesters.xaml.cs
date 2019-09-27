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
    /// Interaction logic for Semesters.xaml
    /// </summary>
    public partial class Semesters : Page
    {
        private readonly SemestersViewModel viewModel;

        public Semesters(ISemesterService semesterService)
        {
            viewModel = new SemestersViewModel(semesterService);

            InitializeComponent();

            this.Loaded += SemestersTable_Load;
        }

        private async void SemestersTable_Load(object sender, RoutedEventArgs e)
        {
            await viewModel.Load();
            DataContext = viewModel;

            this.spinnerGrid.Visibility = Visibility.Hidden;
            this.rootGrid.Visibility = Visibility.Visible;
        }
    }
}
