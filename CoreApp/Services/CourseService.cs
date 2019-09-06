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
    public class CourseService : ICourseService
    {
        private readonly BlokicContext context;

        public CourseService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<bool> CourseInstanceExists(int courseId, int semesterId)
        {
            return await context.CourseInstance.AnyAsync(_ => _.CourseId == courseId && _.SemesterId == semesterId);
        }

        public async Task<List<CourseList>> GetList()
        {
            return await context.Course.Select(_ => new CourseList
            {
                Id = _.Id,
                Name = _.Name
            }).ToListAsync();
        }

        public async Task<List<CourseInstanceList>> GetInastancesList()
        {
            return await context.CourseInstance.Select(_ => new CourseInstanceList
            {
                Course = new CourseList
                {
                    Id = _.CourseId,
                    Name = _.Course.Name
                },
                Semester = new SemesterList
                {
                    Id = _.SemesterId,
                    IsWinter = _.Semester.IsWinter,
                    StartDate = _.Semester.StartDate
                },
                Value = new CourseInstanceBase
                {
                    CourseId = _.CourseId,
                    SemesterId = _.SemesterId
                }
            }).ToListAsync();
        }

        public async Task<List<CourseInstanceList>> GetInastancesListForCourse(int courseId)
        {
            return await context.CourseInstance
                .Where(_ => _.CourseId == courseId)
                .Select(_ => new CourseInstanceList
                {
                    Course = new CourseList
                    {
                        Id = _.CourseId,
                        Name = _.Course.Name
                    },
                    Semester = new SemesterList
                    {
                        Id = _.SemesterId,
                        IsWinter = _.Semester.IsWinter,
                        StartDate = _.Semester.StartDate
                    },
                    Value = new CourseInstanceBase
                    {
                        CourseId = _.CourseId,
                        SemesterId = _.SemesterId
                    }
                }).ToListAsync();
        }

        public async Task<List<CourseUpdate>> GetUpdateable()
        {
            return await context.Course.Select(_ => new CourseUpdate
            {
                Id = _.Id,
                Name = _.Name
            }).ToListAsync();
        }

        public async Task<List<CourseUpdateModel>> GetUpdateableModels()
        {
            return await context.Course
                .Include(_ => _.CourseInstance)
                .ThenInclude(_ => _.Semester)
                .Select(_ => new CourseUpdateModel
                {
                    Id = _.Id,
                    Course = new CourseUpdate
                    {
                        Id = _.Id,
                        Name = _.Name
                    },
                    Instances = _.CourseInstance.Select(ci => new SemesterList
                    {
                        Id = ci.Semester.Id,
                        IsWinter = ci.Semester.IsWinter,
                        StartDate = ci.Semester.StartDate
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<List<CourseInstanceUpdate>> GetInstancesUpdateable()
        {
            return await context.CourseInstance
                .Include(_ => _.Course)
                .Include(_ => _.Semester)
                .Select(_ => new CourseInstanceUpdate
                {
                    Course = new CourseUpdate
                    {
                        Id = _.Course.Id,
                        Name = _.Course.Name
                    },
                    Semester = new SemesterUpdate
                    {
                        Id = _.Semester.Id,
                        StartDate = _.Semester.StartDate,
                        IsWinter = _.Semester.IsWinter,
                        EndDate = _.Semester.EndDate
                    }
                }).ToListAsync();
        }

        public async Task<CourseBase> GetById(int courseId)
        {
            var course = await context.Course.FirstOrDefaultAsync(_ => _.Id == courseId);
            return course != null ? new CourseBase
            {
                Id = course.Id,
                Name = course.Name
            } : null ;
        }

        public async Task<CourseUpdate> CreateBasic(CourseCreate model)
        {
            var course = new Course
            {
                Name = model.Name                
            };

            var errors = course.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            if (context.Course.Any(_ => _.Name == course.Name))
            {
                throw new ValidationException("Course with the same name already exists!");
            }

            await context.Course.AddAsync(course);
            await context.SaveChangesAsync();

            return new CourseUpdate
            {
                Id = course.Id,
                Name = course.Name
            };
        }

        public async Task CreateInstance(int courseId, int semesterId)
        {
            if (!context.Course.Any(_ => _.Id == courseId))
                throw new ValidationException("Requested course doesn't exist.");

            if (!context.Semester.Any(_ => _.Id == semesterId))
                throw new ValidationException("Requested semester doesn't exist.");

            if (context.CourseInstance.Any(_ => _.SemesterId == semesterId && _.CourseId == courseId))
                throw new ValidationException("Course instance already exists.");

            var courseInstance = new CourseInstance
            {
                CourseId = courseId,
                SemesterId = semesterId
            };

            await context.CourseInstance.AddAsync(courseInstance);
            await context.SaveChangesAsync();
        }

        public async Task<CourseUpdate> UpdateBasic(int courseId, CourseUpdate model)
        {
            var course = await context.Course.FirstOrDefaultAsync(_ => _.Id == courseId);
            if (course == null)
                throw new ValidationException("Requested course doesn't exist.");

            course.Name = model.Name;

            var errors = course.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            await context.SaveChangesAsync();

            return new CourseUpdate
            {
                Id = course.Id,
                Name = course.Name
            };
        }

        public async Task Delete(int courseId)
        {
            var course = await context.Course.FirstOrDefaultAsync(_ => _.Id == courseId);
            if (course == null)
                throw new ValidationException("Requested course doesn't exist.");

            if (context.CourseInstance.Any(_ => _.CourseId == courseId))
                throw new ValidationException("Requested course contains active instances!");

            context.Course.Remove(course);
            await context.SaveChangesAsync();
        }

        public async Task DeleteWithInstances(int courseId)
        {
            var course = await context.Course.FirstOrDefaultAsync(_ => _.Id == courseId);
            if (course == null)
                throw new ValidationException("Requested course doesn't exist.");

            var instances = context.CourseInstance.Where(_ => _.CourseId == courseId);

            context.CourseInstance.RemoveRange(instances);
            context.Course.Remove(course);
            await context.SaveChangesAsync();
        }

        public async Task DeleteInstance(int courseId, int semesterId)
        {
            var instance = await context.CourseInstance.FirstOrDefaultAsync(_ => _.SemesterId == semesterId && _.CourseId == courseId);
            if (instance == null)
                throw new ValidationException("Requested course instance doesn't exist.");

            context.CourseInstance.Remove(instance);
            await context.SaveChangesAsync();

        }

    }
}
