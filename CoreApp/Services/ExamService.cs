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
    public class ExamService : IExamService
    {
        private readonly BlokicContext context;

        public ExamService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<Guid> CreateOralExam(OralExamCreate oralExamForm)
        {
            var errors = new List<string>();

            if (!(await context.Course.AnyAsync(_ => _.Id == oralExamForm.CourseId)))
                errors.Add("Odabran je nepostojeći predmet!");

            if (!(await context.Semester.AnyAsync(_ => _.Id == oralExamForm.SemesterId)))
                errors.Add("Odabran je nepostojeći semestar!");

            if (!(await context.CourseInstance.AnyAsync(_ => _.CourseId == oralExamForm.CourseId && _.SemesterId == oralExamForm.SemesterId)))
                errors.Add("Odabrani predmet se ne održava u odabranom semestru!");

            if (!(await context.Enrolment.AnyAsync(_ => _.Id == oralExamForm.EnrolmentId)))
                errors.Add("Odabrani student nije upisan na odabranu instancu predmeta!");

            if (oralExamForm.ExamId.HasValue)
            {
                if (!(await context.Exam.AnyAsync(_ => _.Id == oralExamForm.ExamId.Value)))
                    errors.Add("Odabran je nepostojeći ispit!");
            }
            else if (oralExamForm.Exam == null)
            {
                errors.Add("Nevaljali podaci za kreiranje usmenog ispita!");
            }

            if (context.StudentExam.Any(_ => _.ExamId == oralExamForm.ExamId &&
                   _.EnrolmentId == oralExamForm.EnrolmentId))
            {
                var student = context.StudentExam
                    .Include(_ => _.Enrolment)
                    .ThenInclude(_ => _.Student)
                    .FirstOrDefault(_ => _.ExamId == oralExamForm.ExamId &&
                    _.EnrolmentId == oralExamForm.EnrolmentId)
                    .Enrolment
                    .Student;

                throw new ValidationException(string.Format($"Odabrani student {student.Firstname} {student.Lastname} već je prijavljen na dani ispit."));
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }

            if (!oralExamForm.ExamId.HasValue)
            {
                var exam = new Exam
                {
                    Type = (int)ExamType.Oral,
                    Date = GetDateTime(oralExamForm.Exam.Date, oralExamForm.Exam.Time),
                    CourseId = oralExamForm.CourseId,
                    SemesterId = oralExamForm.SemesterId
                };

                await context.Exam.AddAsync(exam);

                oralExamForm.ExamId = exam.Id;
            }

            if (oralExamForm.ExistingExamDateTime != null)
            {
                var exam = await context.Exam.FirstOrDefaultAsync(_ => _.Id == oralExamForm.ExamId);

                exam.Date = GetDateTime(oralExamForm.ExistingExamDateTime.Date, oralExamForm.ExistingExamDateTime.Time);

                context.Exam.Update(exam);
            }

            var studentExam = new StudentExam
            {
                EnrolmentId = oralExamForm.EnrolmentId,
                ExamId = oralExamForm.ExamId.Value
            };

            await context.StudentExam.AddAsync(studentExam);

            await context.SaveChangesAsync();

            return studentExam.Id;
        }

        public async Task<bool> HasDate(int examId)
        {
            var exam = await context.Exam.FirstOrDefaultAsync(_ => _.Id == examId);

            if (exam != null)
                return exam.Date.HasValue;

            return true;
        }

        public async Task<List<ExamList>> GetOralExamsListForCourseInstance(int courseId, int semesterId)
        {
            return await context.Exam
                .Where(_ => _.Type == (int)ExamType.Oral && _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new ExamList
                {
                    Id = _.Id,
                    CourseId = _.CourseId,
                    Date = _.Date,
                    UserFriendly = Helpers.GetUserFriendlyExam(_.Date, _.CourseInstance.Course.Name, _.CourseInstance.Semester.StartDate, _.CourseInstance.Semester.IsWinter)
                })
                .OrderByDescending(_ => _.Date)
                .ToListAsync();
        }

        public async Task<List<ExamUpdateList>> GetUpdateable()
        {
            return await context.Exam
                .Select(_ => new ExamUpdateList
                {
                    Id = _.Id,
                    Type = (ExamType)_.Type,
                    Exam = new ExamUpdate
                    {
                        Id = _.Id,
                        Date = _.Date,
                        Time = _.Date
                    },
                    Course = new CourseList
                    {
                        Id = _.CourseInstance.CourseId,
                        Name = _.CourseInstance.Course.Name
                    },
                    Semester = new SemesterList
                    {
                        Id = _.SemesterId,
                        IsWinter = _.CourseInstance.Semester.IsWinter,
                        StartDate = _.CourseInstance.Semester.StartDate
                    }
                }).ToListAsync();
        }       

        public async Task<ExamUpdate> Update(int examId, ExamUpdate model)
        {
            var exam = await context.Exam.FirstOrDefaultAsync(_ => _.Id == examId);
            if (exam == null)
                throw new ValidationException("Requested exam doesn't exist.");

            exam.Date = GetDateTime(model.Date, model.Time);

            var errors = exam.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            await context.SaveChangesAsync();

            return new ExamUpdate
            {
                Id = exam.Id,
                Date = exam.Date,
                Time = exam.Date
            };
        }

        //public async Task Create (ExamCreate model)
        //{
        //    var exam = new Exam
        //    {
        //        Type = (int)model.Type,
        //        Date = model.Date,
        //        CourseId = model.CourseInstance.CourseId,
        //        SemesterId = model.CourseInstance.SemesterId
        //    };
        //    ValidateBasic(exam);
        //    await context.Exam.AddAsync(exam);
        //    await context.SaveChangesAsync();
        //}

        public async Task Create(ExamCreate model)
        {
            var exam = new Exam
            {
                Type = (int)model.Type,
                Date = GetDateTime(model.Date, model.Time),
                CourseId = model.CourseInstance.CourseId,
                SemesterId = model.CourseInstance.SemesterId
            };

            ValidateBasic(exam);

            var enrolments = context.Enrolment.Where(_ => model.EnrolmentIds.Contains(_.Id));
            if (model.EnrolmentIds.Count != enrolments.Count())
            {
                throw new ValidationException("Odabran je nepostojeći enrolment.");
            }

            await context.Exam.AddAsync(exam);

            var studentExams = enrolments.Select(_ => new StudentExam
            {
                EnrolmentId = _.Id,
                ExamId = exam.Id
            });

            await context.StudentExam.AddRangeAsync(studentExams);

            await context.SaveChangesAsync();
        }

        public async Task Delete(int examId)
        {
            var exam = await context.Exam.FirstOrDefaultAsync(_ => _.Id == examId);
            if (exam == null)
                throw new ValidationException("Traženi ispit ne psotoji!");

            if (await context.StudentExam.AnyAsync(_ => _.ExamId == examId))
                throw new ValidationException("Brisanje ispita nije moguće jer postoje vezani studentski ispiti.");

            context.Exam.Remove(exam);
            await context.SaveChangesAsync();
        }

        public async Task ForceDelete(int examId)
        {
            var exam = await context.Exam.FirstOrDefaultAsync(_ => _.Id == examId);
            if (exam == null)
                throw new ValidationException("Traženi ispit ne psotoji!");

            var studentExams = context.StudentExam.Where(_ => _.ExamId == examId);

            context.StudentExam.RemoveRange(studentExams);

            context.Exam.Remove(exam);
            await context.SaveChangesAsync();
        }


        private DateTime? GetDateTime(DateTime? date, DateTime? time)
        {
            if (!time.HasValue)
                return date;

            return date.HasValue ? date.Value.Add(time.Value.TimeOfDay) : date;
        }

        private void ValidateBasic(Exam exam)
        {
            var errors = exam.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            if (!context.Course.Any(_ => _.Id == exam.CourseId))
                throw new ValidationException("Traženi predmet ne postoji.");

            if (!context.Semester.Any(_ => _.Id == exam.SemesterId))
                throw new ValidationException("Traženi semestar ne postoji.");

            if (!context.CourseInstance.Any(_ => _.SemesterId == exam.SemesterId && _.CourseId == exam.CourseId))
                throw new ValidationException("Tražena instanca predmeta ne postoji.");

        }
    }
}
