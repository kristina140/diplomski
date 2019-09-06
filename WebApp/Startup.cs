using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Routing;
using Blokic.Models;
using CoreApp.IServices;
using CoreApp.Services;

namespace WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; private set; }
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json");
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddAuthorization();
            services.AddWebEncoders();

            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<ISemesterService, SemesterService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<IEnrolmentService, EnrolmentService>();
            services.AddTransient<IExamService, ExamService>();
            services.AddTransient<IStudentExamService, StudentExamService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddScoped<Services.UtilityService>();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<BlokicContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("LocalConnection"));
                });

            services.AddDotVVM<DotvvmStartup>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            // use DotVVM
            var dotvvmConfiguration = app.UseDotVVM<DotvvmStartup>(env.ContentRootPath);
            dotvvmConfiguration.AssertConfigurationIsValid();
            
            // use static files
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(env.WebRootPath)
            });
        }
    }
}
