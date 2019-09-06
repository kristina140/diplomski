using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.ViewModels.Student
{
    public class StudentCardViewModel : MasterPageViewModel
    {
        private readonly IStudentService _studentService;
        private int StudentId { get; set; }

        public StudentCardViewModel(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Bind(Direction.ServerToClient)]
        public bool ValidId { get; set; }

        [Bind(Direction.ServerToClient)]
        public StudentCard StudentCard { get; set; }

        public bool ShowDescriptions { get; set; } = false;

        [Bind(Direction.ServerToClient)]
        public List<string> ValidationErrors { get; set; }

        public override Task Init()
        {
            if (!Context.Parameters.ContainsKey("id"))
            {
                Context.RedirectToRoute("Basics_Students");
            }
            else
            {
                StudentId = Convert.ToInt32(Context.Parameters["id"]);
            }

            return base.Init();
        }

        public override async Task Load()
        {
            try
            {
                StudentCard = await _studentService.GetStudentCard(StudentId);
                ValidId = true;
            }
            catch (ValidationException ve)
            {
                ValidId = false;
                ValidationErrors = ve.Errors.ToList();
            }

            await base.Load();
        }
    }
}
