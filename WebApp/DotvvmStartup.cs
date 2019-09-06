	using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Services;

namespace WebApp
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {
            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/default.dothtml");

            config.RouteTable.AddGroup("Basics", "Basics", "Views/Basics", table =>
            {
                table.Add("Students", "Students", "Students.dothtml");
                table.Add("Semesters", "Semesters", "Semesters.dothtml");
                table.Add("CourseInstances", "CourseInstances", "CourseInstances.dothtml");
                table.Add("Customers", "Courses", "Courses.dothtml");
            });

            config.RouteTable.Add("Enrolments", "Enrolments", "Views/Enrolments/Enrolments.dothtml");
            config.RouteTable.Add("CreateEnrolments", "Enrolments/Create", "Views/Enrolments/Create.dothtml");

            config.RouteTable.Add("Exams", "Exams", "Views/Exams/Exams.dothtml");
            config.RouteTable.Add("OralExam", "OralExam", "Views/Exams/OralExam.dothtml");
            config.RouteTable.Add("OralExamUpdate", "OralExam/{Id:guid}", "Views/Exams/OralExamUpdate.dothtml");

            config.RouteTable.Add("StudentCard", "Student/{id:int}", "Views/Student/StudentCard.dothtml");

            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));    
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            // register code-only controls and markup controls
            config.Markup.AddCodeControls("cc", typeof(WebApp.Controls.MultiSelect));
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            // register custom resources and adjust paths to the built-in resources
			config.Resources.Register("Styles", new StylesheetResource()
            {
                Location = new UrlResourceLocation("~/styles.css")
            });
        }
        public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.Services.AddScoped<UtilityService>();
            options.AddDefaultTempStorages("temp");
		}
    }
}
