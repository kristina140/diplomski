using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.ViewModels.Exams
{
    public class OralExamUpdateViewModel : MasterPageViewModel
    {
        private readonly IStudentExamService _studentExamService;

        private Guid StudentExamId { get; set; }

        public OralExamUpdateViewModel(
            IStudentExamService studentExamService)
        {
            _studentExamService = studentExamService;
        }

        [Bind(Direction.ServerToClient)]
        public bool ValidId { get; set; }

        [Bind(Direction.ServerToClient)]
        public OralExamList OralExamListData { get; set; }
        [Bind(Direction.ServerToClient)]
        public List<StudentExamList> StudentExams { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<GradeList> GradesList { get; set; }
        [Bind(Direction.ServerToClient)]
        public Grade NoGrade { get; set; } = Grade.NoGrade;

        public OralExamUpdate OralExamData { get; set; } = new OralExamUpdate { };


        [Bind(Direction.ServerToClient)]
        public List<string> ValidationErrors { get; set; }
        [Bind(Direction.ServerToClient)]
        public string Message { get; set; }

        public override Task Init()
        {
            if (!Context.Parameters.ContainsKey("Id"))
            {
                Context.RedirectToRoute("OralExam");
            }
            else
            {
                StudentExamId = (Guid)Context.Parameters["Id"];
            }

            Message = null;
            return base.Init();
        }

        public override async Task Load()
        {
            try
            {
                OralExamListData = await _studentExamService.GetOralExam(StudentExamId);

                if (!Context.IsPostBack)
                {
                    OralExamData.Description = OralExamListData.StudentOralExamUpdate.Description;
                    OralExamData.FinalGrade = OralExamListData.StudentOralExamUpdate.FinalGrade;
                    OralExamData.FinalGradeDate = OralExamListData.StudentOralExamUpdate.FinalGradeDate ?? DateTime.Now;
                    OralExamData.Grade = OralExamListData.StudentOralExamUpdate.Grade;
                    OralExamData.Participated = OralExamListData.StudentOralExamUpdate.Participated;
                    OralExamData.Score = OralExamListData.StudentOralExamUpdate.Score;
                }                

                ValidId = true;
            }
            catch (ValidationException ve)
            {
                ValidId = false;
                ValidationErrors = ve.Errors.ToList();
            }

            if (ValidId)
            {
                StudentExams = await _studentExamService.GetStudentExamList(OralExamListData.Enrolment.Student.Id, StudentExamId, OralExamListData.CourseId);
            }


            GradesList = Enum.GetValues(typeof(Grade)).OfType<Grade>().ToList().Select(_ => new GradeList
            {
                Grade = _,
                Description = _.GetEnumDescription()
            }).ToList();

            await base.Load();
        }

        public override Task PreRender()
        {
            return base.PreRender();
        }

        public async Task Save()
        {
            OralExamData.Id = StudentExamId;
            OralExamData.ExamId = OralExamListData.Exam.Id;
            OralExamData.EnrolmentId = OralExamListData.Enrolment.Id;

            ValidationErrors = null;

            try
            {
                await _studentExamService.UpdateOralExam(OralExamData);
                Message = "Promjene su uspješno spremljene!";
            }
            catch (ValidationPropertyException vpe)
            {
                ValidationErrors = vpe.ErrorsList.ToList();
            }
            catch (ValidationException ve)
            {
                ValidationErrors = ve.Errors.ToList();
            }
        }
    }
}
