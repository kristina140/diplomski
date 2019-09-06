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
    public class StudentService : IStudentService
    {
        private readonly BlokicContext context;

        public StudentService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<List<StudentBase>> GetAll()
        {
            return await context.Student.Select(_ => new StudentBase
            {
                Id = _.Id,
                Firstname = _.Firstname,
                Lastname = _.Lastname,
                IndexNmb = _.IndexNmb,
                Jmbag = _.Jmbag
            }).ToListAsync();
        }

        public async Task<List<StudentList>> GetList()
        {
            return await context.Student.Select(_ => new StudentList
            {
                Id = _.Id,
                Firstname = _.Firstname,
                Lastname = _.Lastname
            }).ToListAsync();
        }

        public async Task<List<StudentUpdate>> GetUpdateable()
        {
            return await context.Student.Select(_ => new StudentUpdate
            {
                Id = _.Id,
                Firstname = _.Firstname,
                Lastname = _.Lastname,
                IndexNmb = _.IndexNmb,
                Jmbag = _.Jmbag
            }).ToListAsync();
        }

        public async Task<StudentBase> GetById(int studentId)
        {
            var student = await context.Student.FirstOrDefaultAsync(_ => _.Id == studentId);
            return student != null ? new StudentBase
            {
                Id = student.Id,
                Firstname = student.Firstname,
                Lastname = student.Lastname,
                IndexNmb = student.IndexNmb,
                Jmbag = student.Jmbag
            } : null;
        }

        public async Task<StudentCard> GetStudentCard(int studentId)
        {
            var student = await context.Student.FirstOrDefaultAsync(_ => _.Id == studentId);
            if (student == null)
                throw new ValidationException("Nepostojeći student!");

            var studentCardCourseEnrolments = await context.StudentExam
                .Include(_ => _.Enrolment)
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .Include(_ => _.Exam)
                .ThenInclude(_ => _.CourseInstance)
                .ThenInclude(_ => _.Course)
                .Where(_ => _.Enrolment.StudentId == studentId)
                .GroupBy(_ => _.Exam.CourseId, (courseId, studExams) => new StudentCardCourseEnrolment
                {
                    Course = new CourseList
                    {
                        Id = courseId,
                        Name = studExams.FirstOrDefault().Exam.CourseInstance.Course.Name
                    },
                    Enrolments = studExams.GroupBy(_ => _.Exam.SemesterId, (semesterId, studentExams) => new StudentCardEnrolment
                    {
                        Semester = new SemesterList
                        {
                            Id = semesterId,
                            IsWinter = studentExams.FirstOrDefault().Exam.CourseInstance.Semester.IsWinter,
                            StartDate = studentExams.FirstOrDefault().Exam.CourseInstance.Semester.StartDate
                        },
                        FinalGrade = studentExams.FirstOrDefault().Enrolment.FinalGrade.ConvertToGrade().GetEnumDescription(),
                        FinalGradeDate = studentExams.FirstOrDefault().Enrolment.GradeDate,
                        StudentExams = studentExams.Select(_ => new StudentExamBase
                        {
                            Description = _.Description,
                            Grade = _.Grade.ConvertToGrade().GetEnumDescription(),
                            Participated = _.Participated,
                            Score = _.Score,

                            ExamDate = _.Exam.Date,
                            ExamType = _.Exam.Type.ConvertToExamType().GetEnumDescription()
                        }).OrderBy(_ => _.ExamDate).ToList()
                    }).OrderBy(_ => _.Semester.StartDate).ToList()
                }).ToListAsync();

            return new StudentCard
            {
                Student = new StudentBase
                {
                    Id = student.Id,
                    Firstname = student.Firstname,
                    Lastname = student.Lastname,
                    IndexNmb = student.IndexNmb,
                    Jmbag = student.Jmbag
                },
                CourseEnrolments = studentCardCourseEnrolments
            };
        }

        public async Task<StudentUpdate> Create(StudentCreate model)
        {
            var student = new Student
            {
                Firstname = model.Firstname,
                Lastname = model.Lastname,
                Jmbag = model.Jmbag,
                IndexNmb = model.IndexNmb
            };

            var errors = student.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            await context.Student.AddAsync(student);
            await context.SaveChangesAsync();

            return new StudentUpdate
            {
                Id = student.Id,
                Firstname = student.Firstname,
                Lastname = student.Lastname,
                Jmbag = student.Jmbag,
                IndexNmb = student.IndexNmb
            };
        }

        public async Task<List<StudentUpdate>> Create(List<StudentCreate> models)
        {
            var students = models.Select(_ => new Student
            {
                Firstname = _.Firstname,
                Lastname = _.Lastname,
                Jmbag = _.Jmbag,
                IndexNmb = _.IndexNmb
            }).ToList();

            students.ForEach(student =>
            {
                var errors = student.Validate();
                if (errors.Any())
                    throw new ValidationPropertyException(errors);
            });

            await context.Student.AddRangeAsync(students);
            await context.SaveChangesAsync();

            return students.Select(_ => new StudentUpdate
            {
                Id = _.Id,
                Firstname = _.Firstname,
                Lastname = _.Lastname,
                Jmbag = _.Jmbag,
                IndexNmb = _.IndexNmb
            }).ToList();
        }

        public async Task<StudentUpdate> UpdateBasic(int studentId, StudentUpdate model)
        {
            var student = await context.Student.FirstOrDefaultAsync(_ => _.Id == studentId);
            if (student == null)
                throw new ValidationException("Requested student doesn't exist.");

            student.Firstname = model.Firstname;
            student.Lastname = model.Lastname;
            student.Jmbag = model.Jmbag;
            student.IndexNmb = model.IndexNmb;

            var errors = student.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            await context.SaveChangesAsync();

            return new StudentUpdate
            {
                Id = student.Id,
                Firstname = student.Firstname,
                Lastname = student.Lastname,
                Jmbag = student.Jmbag,
                IndexNmb = student.IndexNmb
            } ;
        }

        public async Task Delete(int studentId)
        {
            var student = await context.Student.FirstOrDefaultAsync(_ => _.Id == studentId);
            if (student == null)
                throw new ValidationException("Requested student doesn't exist.");

            context.Student.Remove(student);
            await context.SaveChangesAsync();
        }
    }
}
