using CoreApp;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.ViewModels.Enrolments
{
    public class CreateViewModel : MasterPageViewModel
    {
        private readonly IEnrolmentService _enrolmentService;
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;

        public CreateViewModel(
            IEnrolmentService enrolmentService,
            IStudentService studentService,
            ICourseService courseService)
        {
            _enrolmentService = enrolmentService;
            _studentService = studentService;
            _courseService = courseService;
        }

        public List<EnrolmentCreateListModel> NewEnrolments { get; set; } = new List<EnrolmentCreateListModel>() { };

        [Bind(Direction.ServerToClient)]
        public List<GradeList> GradesList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<StudentBase> StudentsList { get; set; }
        [Bind(Direction.ServerToClient)]
        public List<CourseInstanceList> CourseInstancesList { get; set; }

        [Bind(Direction.ServerToClient)]
        public List<string> ValidationErrors { get; set; }

        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<CourseInstanceBase> SelectedCourseInstances { get; set; }

        public override async Task Load()
        {
            StudentsList = await _studentService.GetAll();
            CourseInstancesList = await _courseService.GetInastancesList();

            GradesList = Enum.GetValues(typeof(Grade)).OfType<Grade>().ToList().Select(_ => new GradeList
            {
                Grade = _,
                Description = _.GetEnumDescription()
            }).ToList();

            await base.Load();
        }

        public void Add()
        {
            ValidationErrors = null;

            var duplicates = new List<string>();

            foreach (var studentId in SelectedStudents)
            {
                var student = StudentsList.FirstOrDefault(_ => _.Id == studentId);

                foreach (var courseInstanceIds in SelectedCourseInstances)
                {
                    var courseInstance = CourseInstancesList.FirstOrDefault(_ => _.Value.SemesterId == courseInstanceIds.SemesterId &&
                        _.Value.CourseId == courseInstanceIds.CourseId);

                    if (NewEnrolments.Any(_ => _.Student.Id == student.Id && 
                        _.Semester.Id == courseInstance.Semester.Id &&  
                        _.Course.Id == courseInstance.Course.Id))
                    {
                        duplicates.Add(string.Format($"{student.UserFriendly} - {courseInstance.UserFriendly}"));
                        continue;
                    }

                    NewEnrolments.Add(new EnrolmentCreateListModel
                    {
                        Student = student,
                        Course = courseInstance.Course,
                        Semester = courseInstance.Semester,
                        Enrolment = new EnrolmentCreate
                        {
                            StudentId = student.Id,
                            CourseInstance = new CourseInstanceBase
                            {
                                CourseId = courseInstance.Course.Id,
                                SemesterId = courseInstance.Semester.Id
                            }
                        }
                    });
                }

            }

            if (duplicates.Any())
            {
                ValidationErrors = new List<string>
                {
                    "Duplikati :"
                };

                duplicates.ForEach(_ => ValidationErrors.Add(_));

                ValidationErrors.Add("nisu dodani ponovno.");
            }
        }

        public void Remove(EnrolmentCreateListModel item)
        {
            var removed = NewEnrolments.Remove(item);
        }

        public async Task SafeSave()
        {
            ValidationErrors = null;
            try
            {
                await _enrolmentService.CreateSafe(NewEnrolments.Select(_ => _.Enrolment).ToList());
                Context.RedirectToRoute("Enrolments");
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

        public async Task ForceSave()
        {
            ValidationErrors = null;
            try
            {
                await _enrolmentService.UpdateAndCreate(NewEnrolments.Select(_ => _.Enrolment).ToList());
                Context.RedirectToRoute("Enrolments");
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
