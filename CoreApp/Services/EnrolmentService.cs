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
    public class EnrolmentService : IEnrolmentService
    {
        private readonly BlokicContext context;

        public EnrolmentService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<List<EnrolmentBase>> GetAll()
        {
            return await context.Enrolment
                .Include(_ => _.CourseInstance).ThenInclude(_ => _.Course)
                .Select(_ => new EnrolmentBase
                {
                    Id = _.Id,
                    Course = new CourseBase
                    {
                        Id = _.CourseInstance.Course.Id,
                        Name = _.CourseInstance.Course.Name
                    },
                    Semester = new SemesterBase
                    {
                        Id = _.CourseInstance.Semester.Id,
                        IsWinter = _.CourseInstance.Semester.IsWinter,
                        StartDate = _.CourseInstance.Semester.StartDate,
                        EndDate = _.CourseInstance.Semester.EndDate
                    },
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        Lastname = _.Student.Lastname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag
                    },
                    GradeDate = _.GradeDate,
                    FinalGrade = _.FinalGrade.ConvertToGrade()
                }).ToListAsync();
        }

        public async Task<List<EnrolmentList>> GetForCourseInstance(int courseId, int semesterId)
        {
            return await context.Enrolment.Where(_ => _.CourseId == courseId && _.SemesterId == semesterId)
                .Select(_ => new EnrolmentList
                {
                    Id = _.Id,
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        Lastname = _.Student.Lastname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag
                    }
                }).ToListAsync();
        }

        public async Task<List<EnrolmentList>> GetForCourseInstance(CourseInstanceBase courseInstance)
        {
            return await GetForCourseInstance(courseInstance.CourseId, courseInstance.SemesterId);
        }

        public async Task<List<EnrolmentList>> GetAvailableForCourseInstance(CourseInstanceBase courseInstance, int examId)
        {
            return await context.Enrolment
                .Where(_ => _.CourseId == courseInstance.CourseId && 
                    _.SemesterId == courseInstance.SemesterId &&
                    !_.StudentExam.Any(__ => __.ExamId == examId))
                .Select(_ => new EnrolmentList
                {
                    Id = _.Id,
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        Lastname = _.Student.Lastname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag
                    }
                }).ToListAsync();
        }

        public async Task<List<EnrolmentUpdateList>> GetUpdateable()
        {
            return await context.Enrolment
                .Include(_ => _.CourseInstance).ThenInclude(_ => _.Course)
                .Select(_ => new EnrolmentUpdateList
                {
                    Id = _.Id,
                    Course = new CourseBase
                    {
                        Id = _.CourseInstance.Course.Id,
                        Name = _.CourseInstance.Course.Name
                    },
                    Semester = new SemesterBase
                    {
                        Id = _.CourseInstance.Semester.Id,
                        IsWinter = _.CourseInstance.Semester.IsWinter,
                        StartDate = _.CourseInstance.Semester.StartDate,
                        EndDate = _.CourseInstance.Semester.EndDate
                    },
                    Student = new StudentBase
                    {
                        Id = _.Student.Id,
                        Firstname = _.Student.Firstname,
                        Lastname = _.Student.Lastname,
                        IndexNmb = _.Student.IndexNmb,
                        Jmbag = _.Student.Jmbag
                    },
                    Enrolment = new EnrolmentUpdate
                    {
                        Id = _.Id,
                        CourseInstance = new CourseInstanceBase
                        {
                            SemesterId = _.SemesterId,
                            CourseId = _.SemesterId
                        },
                        StudentId = _.StudentId,
                        FinalGrade = _.FinalGrade.ConvertToGrade(),
                        GradeDate = _.GradeDate
                    }
                }).ToListAsync();
        }

        public async Task<EnrolmentUpdate> Create(EnrolmentCreate model)
        {
            var enrolment = new Enrolment
            {
                SemesterId = model.CourseInstance.SemesterId,
                CourseId = model.CourseInstance.CourseId,
                StudentId = model.StudentId,
                FinalGrade = model.FinalGrade.ConvertGrade(),
                GradeDate = model.GradeDate
            };

            Validate(enrolment);

            await context.Enrolment.AddAsync(enrolment);
            await context.SaveChangesAsync();

            return new EnrolmentUpdate
            {
                Id = enrolment.Id,
                CourseInstance = new CourseInstanceBase
                {
                    CourseId = enrolment.CourseId,
                    SemesterId = enrolment.SemesterId
                },
                StudentId = enrolment.StudentId,
                FinalGrade = enrolment.FinalGrade.ConvertToGrade(),
                GradeDate = enrolment.GradeDate
            };
        }

        public async Task<EnrolmentUpdate> CreateBase(EnrolmentBaseCreate model)
        {
            var enrolment = new Enrolment
            {
                SemesterId = model.CourseInstance.SemesterId,
                CourseId = model.CourseInstance.CourseId,
                StudentId = model.StudentId
            };

            Validate(enrolment);

            await context.Enrolment.AddAsync(enrolment);
            await context.SaveChangesAsync();

            return new EnrolmentUpdate
            {
                Id = enrolment.Id,
                CourseInstance = new CourseInstanceBase
                {
                    CourseId = enrolment.CourseId,
                    SemesterId = enrolment.SemesterId
                },
                StudentId = enrolment.StudentId,
                FinalGrade = enrolment.FinalGrade.ConvertToGrade(),
                GradeDate = enrolment.GradeDate
            };
        }

        public async Task<List<EnrolmentUpdate>> CreateSafe(List<EnrolmentCreate> models)
        {
            var enrolments = GetNewEnrolments(models, context.Enrolment.AsNoTracking()) ;
            enrolments.ToList().ForEach(enrolment => ValidateBasic(enrolment));

            await context.Enrolment.AddRangeAsync(enrolments);
            await context.SaveChangesAsync();

            return enrolments.Select(enrolment => new EnrolmentUpdate
            {
                Id = enrolment.Id,
                CourseInstance = new CourseInstanceBase
                {
                    CourseId = enrolment.CourseId,
                    SemesterId = enrolment.SemesterId
                },
                StudentId = enrolment.StudentId,
                FinalGrade = enrolment.FinalGrade.ConvertToGrade(),
                GradeDate = enrolment.GradeDate
            }).ToList();
        }

        public async Task UpdateAndCreate(List<EnrolmentCreate> models)
        {
            var newEnrolments = new List<Enrolment>();
            var existing = new List<Enrolment>();

            foreach (var model in models)
            {
                var enrolment = await context.Enrolment.FirstOrDefaultAsync(_ =>
                    _.CourseId == model.CourseInstance.CourseId &&
                    _.SemesterId == model.CourseInstance.SemesterId &&
                    _.StudentId == model.StudentId);

                if (enrolment != null)
                {
                    enrolment.GradeDate = model.GradeDate;
                    enrolment.FinalGrade = model.FinalGrade.ConvertGrade();

                    Validate(enrolment);

                    existing.Add(enrolment);
                }
                else
                {
                    var newEnrolment = new Enrolment
                    {
                        SemesterId = model.CourseInstance.SemesterId,
                        CourseId = model.CourseInstance.CourseId,
                        StudentId = model.StudentId,
                        FinalGrade = model.FinalGrade.ConvertGrade(),
                        GradeDate = model.GradeDate
                    };

                    Validate(newEnrolment);

                    newEnrolments.Add(newEnrolment);
                }

            }

            context.Enrolment.UpdateRange(existing);
            newEnrolments.ForEach(async _ => await context.Enrolment.AddAsync(_));
            
            await context.SaveChangesAsync();
        }

        public async Task<EnrolmentUpdate> Update(int enrolmentId, EnrolmentUpdate model)
        {
            var enrolment = await context.Enrolment.FirstOrDefaultAsync(_ => _.Id == enrolmentId);
            if (enrolment == null)
                throw new ValidationException("Requested enrolment doesn't exist.");

            enrolment.FinalGrade = model.FinalGrade.ConvertGrade();
            enrolment.GradeDate = model.GradeDate;

            var errors = enrolment.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            await context.SaveChangesAsync();

            return new EnrolmentUpdate
            {
                Id = enrolment.Id,
                CourseInstance = new CourseInstanceBase
                {
                    SemesterId = enrolment.SemesterId,
                    CourseId = enrolment.CourseId
                },
                StudentId = enrolment.StudentId,
                FinalGrade = enrolment.FinalGrade.ConvertToGrade(),
                GradeDate = enrolment.GradeDate
            };
        }

        public async Task Delete(int enrolmentId)
        {
            var enrolment = await context.Enrolment.FirstOrDefaultAsync(_ => _.Id == enrolmentId);
            if (enrolment == null)
                throw new ValidationException("Requested enrolment doesn't exist.");

            context.Enrolment.Remove(enrolment);
            await context.SaveChangesAsync();
        }


        private void ValidateBasic(Enrolment enrolment)
        {
            var errors = enrolment.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            if (!context.Semester.Any(_ => _.Id == enrolment.SemesterId))
                throw new ValidationException("Requested semester doesn't exist.");

            if (!context.Course.Any(_ => _.Id == enrolment.CourseId))
                throw new ValidationException("Requested course doesn't exist.");

            if (!context.Student.Any(_ => _.Id == enrolment.StudentId))
                throw new ValidationException("Requested student doesn't exist.");

            if (!context.CourseInstance.Any(_ => _.CourseId == enrolment.CourseId && _.SemesterId == enrolment.SemesterId))
                throw new ValidationException("Requested course instance doesn't exist.");

        }

        private void Validate(Enrolment enrolment)
        {
            ValidateBasic(enrolment);

            if (context.Enrolment.Any(_ => _.Id != enrolment.Id &&
                    _.SemesterId == enrolment.SemesterId &&
                    _.StudentId == enrolment.StudentId &&
                    _.CourseId == enrolment.CourseId)
               )
            {
                throw new ValidationException("Selected student is already enrolled on this course in selected semester.");
            }
        }

        private IEnumerable<Enrolment> GetNewEnrolments(List<EnrolmentCreate> models, IQueryable<Enrolment> enrolments)
        {
            foreach (var model in models)
            {
                if (!enrolments.Any(enrolment =>
                   enrolment.CourseId == model.CourseInstance.CourseId &&
                   enrolment.SemesterId == model.CourseInstance.SemesterId &&
                   enrolment.StudentId == model.StudentId))
                    yield return new Enrolment
                    {
                        SemesterId = model.CourseInstance.SemesterId,
                        CourseId = model.CourseInstance.CourseId,
                        StudentId = model.StudentId,
                        FinalGrade = model.FinalGrade.ConvertGrade(),
                        GradeDate = model.GradeDate
                    };
            }
        }
    }
}
