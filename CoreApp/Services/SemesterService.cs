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
    public class SemesterService : ISemesterService
    {
        private readonly BlokicContext context;

        public SemesterService(BlokicContext blokicContext)
        {
            context = blokicContext;
        }

        public async Task<List<SemesterBase>> GetAll()
        {
            return await context.Semester.Select(_ => new SemesterBase
            {
                Id = _.Id,
                StartDate = _.StartDate,
                EndDate = _.EndDate,
                IsWinter = _.IsWinter
            }).ToListAsync();
        }

        public async Task<List<SemesterList>> GetList()
        {
            return await context.Semester.Select(_ => new SemesterList
            {
                Id = _.Id,
                StartDate = _.StartDate,
                IsWinter = _.IsWinter
            })
            .OrderByDescending(_ => _.StartDate)
            .ToListAsync();
        }

        public async Task<List<SemesterUpdate>> GetUpdateable()
        {
            return await context.Semester.Select(_ => new SemesterUpdate
            {
                Id = _.Id,
                StartDate = _.StartDate,
                EndDate = _.EndDate,
                IsWinter = _.IsWinter
            }).ToListAsync();
        }

        public async Task<SemesterBase> GetById(int semesterId)
        {
            var semester = await context.Semester.FirstOrDefaultAsync(_ => _.Id == semesterId);
            return semester != null ? new SemesterBase
            {
                Id = semester.Id,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                IsWinter = semester.IsWinter
            } : null;
        }

        public async Task<SemesterUpdate> Create(SemesterCreate model)
        {
            var semester = new Semester { };
            UpdateValues(semester, model);
            Validate(semester);

            await context.Semester.AddAsync(semester);
            await context.SaveChangesAsync();

            return new SemesterUpdate
            {
                Id = semester.Id,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                IsWinter = semester.IsWinter
            };
        }

        public async Task<List<SemesterUpdate>> Create(List<SemesterCreate> models)
        {
            var semesters = models.Select(_ =>
            {
                var semester = new Semester { };
                UpdateValues(semester, _);
                Validate(semester);

                return semester;
            });

            await context.Semester.AddRangeAsync(semesters);
            await context.SaveChangesAsync();

            return semesters.Select(_ => new SemesterUpdate
            {
                Id = _.Id,
                StartDate = _.StartDate,
                EndDate = _.EndDate,
                IsWinter = _.IsWinter
            }).ToList();
        }

        public async Task<SemesterUpdate> UpdateBasic(int semesterId, SemesterUpdate model)
        {
            var semester = await context.Semester.FirstOrDefaultAsync(_ => _.Id == semesterId);
            if (semester == null)
                throw new ValidationException("Requested semester doesn't exist.");

            UpdateValues(semester, model);
            Validate(semester);

            await context.SaveChangesAsync();

            return new SemesterUpdate
            {
                Id = semester.Id,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                IsWinter = semester.IsWinter
            } ;
        }

        public async Task Delete(int semesterId)
        {
            var semester = await context.Semester.FirstOrDefaultAsync(_ => _.Id == semesterId);
            if (semester == null)
                throw new ValidationException("Requested semester doesn't exist.");

            context.Semester.Remove(semester);
            await context.SaveChangesAsync();
        }

        private void UpdateValues (Semester semester, SemesterCreate model)
        {
            semester.StartDate = model.StartDate;
            semester.EndDate = model.EndDate;
            semester.IsWinter = model.IsWinter;
        }

        private void Validate (Semester semester)
        {
            var errors = semester.Validate();
            if (errors.Any())
                throw new ValidationPropertyException(errors);

            if (context.Semester.Any(_ => _.Id != semester.Id && 
                    _.StartDate.Year == semester.StartDate.Year &&
                    _.IsWinter == semester.IsWinter )
               )
            {
                throw new ValidationException("This semester already exists.");
            }
        }
    }
}
