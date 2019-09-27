using Blokic.Models;
using CoreApp.BusinessModels;
using CoreApp.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreApp.Services
{
    public class StudentExamService : IStudentExamService
    {
        private readonly BlokicContext context;

        public StudentExamService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<ExistingStudentOralExam> GetStudentOralExam(int examId, int enrolmentId)
        {
            var studentExam = await context.StudentExam
                .Include(_ => _.Enrolment)
                .ThenInclude(_ => _.Student)
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Course)
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .FirstOrDefaultAsync(_ => _.ExamId == examId && _.EnrolmentId == enrolmentId);

            return studentExam != null ? new ExistingStudentOralExam
            {
                Id = studentExam.Id,
                Student = Helpers.GetUserFriendlyStudent(
                    studentExam.Enrolment.Student.Firstname,
                    studentExam.Enrolment.Student.Lastname,
                    studentExam.Enrolment.Student.Jmbag,
                    studentExam.Enrolment.Student.IndexNmb),
                Exam = Helpers.GetUserFriendlyExam(
                        studentExam.Exam.Date,
                        studentExam.Exam.CourseInstance.Course.Name,
                        studentExam.Exam.CourseInstance.Semester.StartDate,
                        studentExam.Exam.CourseInstance.Semester.IsWinter)
            } : null ;
        }

        public async Task<OralExamList> GetOralExam(Guid id)
        {
            var studentExam = await context.StudentExam
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Course)
                .Include(_ => _.Enrolment)
                .ThenInclude(_ => _.Student)
                .FirstOrDefaultAsync(_ => _.Id == id);

            if (studentExam == null)
                throw new ValidationException("Ne postoji traženi studentski ispit.");

            return new OralExamList
            {
                Exam = new ExamBase
                {
                    Id = studentExam.ExamId,
                    UserFriendly = Helpers.GetUserFriendlyExam(
                        studentExam.Exam.Date,
                        studentExam.Exam.CourseInstance.Course.Name,
                        studentExam.Exam.CourseInstance.Semester.StartDate,
                        studentExam.Exam.CourseInstance.Semester.IsWinter)
                },
                Enrolment = new EnrolmentList
                {
                    Id = studentExam.EnrolmentId,
                    Student = new StudentBase
                    {
                        Id = studentExam.Enrolment.StudentId,
                        Firstname = studentExam.Enrolment.Student.Firstname,
                        Lastname = studentExam.Enrolment.Student.Lastname,
                        IndexNmb = studentExam.Enrolment.Student.IndexNmb,
                        Jmbag = studentExam.Enrolment.Student.Jmbag
                    }
                },
                Course = studentExam.Exam.CourseInstance.Course.Name,
                CourseId = studentExam.Exam.CourseId,
                StudentOralExamUpdate = new OralExamUpdate
                {
                    Id = studentExam.Id,
                    EnrolmentId = studentExam.EnrolmentId,
                    ExamId = studentExam.ExamId,

                    Description = studentExam.Description,
                    Grade = studentExam.Grade.ConvertToGrade(),
                    Participated = studentExam.Participated,
                    Score = studentExam.Score,

                    FinalGrade = studentExam.Enrolment.FinalGrade.ConvertToGrade(),
                    FinalGradeDate = studentExam.Enrolment.GradeDate
                }
            };

        }

        public async Task<List<StudentExamBaseList>> GetStudentExamList()
        {
            return await context.StudentExam.Select(_ => new StudentExamBaseList
            {
                Participated = _.Participated,
                Score = _.Score,
                Grade = _.Grade.ConvertToGrade().GetEnumDescription(),
                Description = _.Description,

                ExamType = ((ExamType)_.Exam.Type).GetEnumDescription(),
                ExamDate = _.Exam.Date,

                Semester = _.Exam.CourseInstance.Semester.StartDate.GetUserFriendlySemester(_.Exam.CourseInstance.Semester.IsWinter)
            }).ToListAsync();
        }

        public async Task<List<StudentExamList>> GetStudentExamList(int studentId, Guid studentExamId, int courseId)
        {
            return await context.StudentExam
                .Where(_ => _.Enrolment.StudentId == studentId && _.Id != studentExamId && _.Exam.CourseId == courseId)
                .Select(_ => new StudentExamList
                {
                    Participated = _.Participated,
                    Score = _.Score,
                    Grade = _.Grade.ConvertToGrade().GetEnumDescription(),
                    Description = _.Description,

                    ExamType = _.Exam.Type.ConvertToExamType().GetEnumDescription(),
                    ExamDate = _.Exam.Date,

                    Semester = _.Exam.CourseInstance.Semester.StartDate.GetUserFriendlySemester(_.Exam.CourseInstance.Semester.IsWinter),
                    Course = _.Exam.CourseInstance.Course.Name
                })
                .OrderByDescending(_ => _.ExamDate)
                .ToListAsync();
        }

        public async Task<List<StudentExamUpdateList>> GetStudentExamsForExam(int examId)
        {
            return await context.StudentExam.Where(_ => _.ExamId == examId)
                .Select(_ => new StudentExamUpdateList
                {
                    Student = new StudentBase
                    {
                        Id = _.Enrolment.Student.Id,
                        Firstname = _.Enrolment.Student.Firstname,
                        Lastname = _.Enrolment.Student.Lastname,
                        IndexNmb = _.Enrolment.Student.IndexNmb,
                        Jmbag = _.Enrolment.Student.Jmbag
                    },
                    StudentExam = new StudentExamUpdate
                    {
                        ExamId = _.ExamId,
                        Description = _.Description,
                        EnrolmentId = _.EnrolmentId,
                        Grade = _.Grade.ConvertToGrade(),
                        Participated = _.Participated,
                        Score = _.Score
                    },
                    StudentExamId = _.Id
                }).ToListAsync();
        }

        public async Task<StudentExamsExport> GetStudentExamsExport(int courseId, int semesterId)
        {
            var courseInstance = await context.CourseInstance
                .Include(_ => _.Course)
                .Include(_ => _.Semester)
                .FirstOrDefaultAsync(_ => _.CourseId == courseId && _.SemesterId == semesterId);

            if (courseInstance == null)
                return null;
            
            var exams = await context.Exam
                .Include(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .Where(_ => _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new ExamExport
                {
                    ExamId = _.Id,
                    ExamDate = _.Date,
                    ExamType = _.Type.ConvertToExamType(),
                    Semestar = new SemesterList
                    {
                        Id = _.CourseInstance.Semester.Id,
                        IsWinter = _.CourseInstance.Semester.IsWinter,
                        StartDate = _.CourseInstance.Semester.StartDate
                    }
                }).ToListAsync();

            var students = await context.Enrolment
                .Include(_ => _.Student)
                .Where(_ => _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new EnrolmentExport
                {
                    FinalGrade = _.FinalGrade,
                    FinalGradeDate = _.GradeDate,
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag,
                        Lastname = _.Student.Lastname
                    }
                }).ToListAsync();

            var studentExams = await context.StudentExam
                .Include(_ => _.Enrolment)
                .Where(_ => _.Enrolment.CourseId == courseId && _.Enrolment.SemesterId == semesterId)
                .Select(_ => new StudentExamExport
                {
                    ExamId = _.ExamId,
                    Grade = _.Grade,
                    Participated = _.Participated,
                    Score = _.Score,
                    StudentExamId = _.Id,
                    StudentId = _.Enrolment.StudentId
                }).ToListAsync();

            return new StudentExamsExport
            {
                Exams = exams,
                Students = students,
                StudentExams = studentExams.ToDictionary(_ => new Tuple<int, int>(_.ExamId, _.StudentId), _ => _),
                Course = courseInstance.Course.Name,
                Semester = Helpers.GetUserFriendlySemester(courseInstance.Semester.StartDate, courseInstance.Semester.IsWinter)
            };
        }

        public StudentExamsExport GetStudentExamsExportSync(int courseId, int semesterId)
        {
            var courseInstance = context.CourseInstance
                .Include(_ => _.Course)
                .Include(_ => _.Semester)
                .FirstOrDefault(_ => _.CourseId == courseId && _.SemesterId == semesterId);

            if (courseInstance == null)
                return null;

            var exams = context.Exam
                .Include(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .Where(_ => _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new ExamExport
                {
                    ExamId = _.Id,
                    ExamDate = _.Date,
                    ExamType = _.Type.ConvertToExamType(),
                    Semestar = new SemesterList
                    {
                        Id = _.CourseInstance.Semester.Id,
                        IsWinter = _.CourseInstance.Semester.IsWinter,
                        StartDate = _.CourseInstance.Semester.StartDate
                    }
                }).ToList();

            var students = context.Enrolment
                .Include(_ => _.Student)
                .Where(_ => _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new EnrolmentExport
                {
                    FinalGrade = _.FinalGrade,
                    FinalGradeDate = _.GradeDate,
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag,
                        Lastname = _.Student.Lastname
                    }
                }).ToList();

            var studentExams = context.StudentExam
                .Include(_ => _.Enrolment)
                .Where(_ => _.Enrolment.CourseId == courseId && _.Enrolment.SemesterId == semesterId)
                .Select(_ => new StudentExamExport
                {
                    ExamId = _.ExamId,
                    Grade = _.Grade,
                    Participated = _.Participated,
                    Score = _.Score,
                    StudentExamId = _.Id,
                    StudentId = _.Enrolment.StudentId
                }).ToList();

            return new StudentExamsExport
            {
                Exams = exams,
                Students = students,
                StudentExams = studentExams.ToDictionary(_ => new Tuple<int, int>(_.ExamId, _.StudentId), _ => _),
                Course = courseInstance.Course.Name,
                Semester = Helpers.GetUserFriendlySemester(courseInstance.Semester.StartDate, courseInstance.Semester.IsWinter)
            };
        }

        public async Task Create(List<StudentExamCreate> models)
        {
            var studentExams = models.Select(_ => new StudentExam
            {
                EnrolmentId = _.EnrolmentId,
                ExamId = _.ExamId
            }).ToList();

            studentExams.ForEach(_ => Validate(_));

            await context.StudentExam.AddRangeAsync(studentExams);
            await context.SaveChangesAsync();
        }

        public async Task Create(StudentExamsCreate model)
        {
            var studentExams = model.EnrolmentIds.Select(_ => new StudentExam
            {
                EnrolmentId = _,
                ExamId = model.ExamId
            }).ToList();

            studentExams.ForEach(_ => Validate(_));

            await context.StudentExam.AddRangeAsync(studentExams);
            await context.SaveChangesAsync();
        }

        public async Task UpdateOralExam(OralExamUpdate model)
        {
            var studentExam = await context.StudentExam
                .Include(_ => _.Enrolment)
                .FirstOrDefaultAsync(_ => _.Id == model.Id && _.ExamId == model.ExamId && _.EnrolmentId == model.EnrolmentId);
            if (studentExam == null)
                throw new ValidationException("Ne postoji traženi studentski ispit.");

            studentExam.Description = model.Description;
            studentExam.Score = model.Score;
            studentExam.Grade = model.Grade.ConvertGrade();
            studentExam.Participated = model.Participated;

            studentExam.Enrolment.FinalGrade = model.FinalGrade.ConvertGrade();
            studentExam.Enrolment.GradeDate = model.FinalGrade.HasValue && model.FinalGrade.Value != Grade.NoGrade ?
                model.FinalGradeDate : null;

            var errorsStudentExam = studentExam.Validate();
            var errorsEnrolment = studentExam.Enrolment.Validate();

            if (errorsStudentExam.Any() || errorsEnrolment.Any())
                throw new ValidationPropertyException(errorsStudentExam.Concat(errorsEnrolment).ToDictionary(_ => _.Key, _ => _.Value));

            context.StudentExam.Update(studentExam);
            await context.SaveChangesAsync();
        }

        public async Task Update(StudentExamUpdate model)
        {
            await UpdateStudentExam(model);

            await context.SaveChangesAsync();
        }

        public async Task Update(List<StudentExamUpdate> models)
        {
            foreach (var model in models)
            {
                await UpdateStudentExam(model);
            }

            await context.SaveChangesAsync();
        }

        public async Task Delete(int enrolmentId, int examId)
        {
            var studentExam = await context.StudentExam.FirstOrDefaultAsync(_ => _.EnrolmentId == enrolmentId && _.ExamId == examId);
            if (studentExam == null)
                throw new ValidationException("Requested studentexam doesn't exist.");

            context.StudentExam.Remove(studentExam);
            await context.SaveChangesAsync();
        }


        private async Task UpdateStudentExam(StudentExamUpdate model)
        {
            if (model == null)
                throw new ValidationException("Nevažeći podaci.");

            var studentExam = await context.StudentExam.FirstOrDefaultAsync(_ => _.ExamId == model.ExamId && _.EnrolmentId == model.EnrolmentId);
            if (studentExam == null)
                throw new ValidationException("Ne postoji traženi studentski ispit.");

            studentExam.Description = model.Description;
            studentExam.Grade = model.Grade.ConvertGrade();
            studentExam.Participated = model.Participated;
            studentExam.Score = model.Score;

            var errors = studentExam.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            context.StudentExam.Update(studentExam);
        }

        private void ValidateBasic(StudentExam studentExam)
        {
            var errors = studentExam.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            if (!context.Exam.Any(_ => _.Id == studentExam.ExamId))
                throw new ValidationException("Dani ispit ne postoji.");

            if (!context.Enrolment.Any(_ => _.Id == studentExam.EnrolmentId))
                throw new ValidationException("Dani enrolment ne postoji.");

        }

        private void Validate(StudentExam studentExam)
        {
            ValidateBasic(studentExam);

            if (context.StudentExam.Any(_ => _.ExamId == studentExam.ExamId &&
                    _.EnrolmentId == studentExam.EnrolmentId))
            {
                var student = context.StudentExam
                    .Include(_ => _.Enrolment)
                    .ThenInclude(_ => _.Student)
                    .FirstOrDefault(_ => _.ExamId == studentExam.ExamId &&
                    _.EnrolmentId == studentExam.EnrolmentId)
                    .Enrolment
                    .Student;

                throw new ValidationException(string.Format($"Odabrani student {student.Firstname} {student.Lastname} već je prijavljen na dani ispit."));
            }
        }

    }
}
