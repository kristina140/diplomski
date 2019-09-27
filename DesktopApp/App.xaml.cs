using Blokic.Models;
using CoreApp.IServices;
using CoreApp.Services;
using DesktopApp.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ISemesterService, SemesterService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IEnrolmentService, EnrolmentService>();
            services.AddTransient<IExamService, ExamService>();
            services.AddTransient<IStudentExamService, StudentExamService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddTransient<IDownloadFileService, DownloadFileService>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<BlokicContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("LocalConnection"));
                });

            services.AddTransient(typeof(MainWindow));
        }
    }
}
