using Blokic.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blokic.Repositories
{
    public class StudentRepository
    {
        private BlokicContext _context { get; set; }

        public StudentRepository(BlokicContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Student.ToListAsync();
        }

        public async Task<Student> GetById(int studentId)
        {
            return await _context.Student.FirstOrDefaultAsync(_ => _.Id == studentId);
        }

        public async Task Create (Student model)
        {
            var errors = model.Validate();
            if (errors.Any())
                throw new ValidationException(errors);

            await _context.Student.AddAsync(model);
            await _context.SaveChangesAsync();
        }

    }
}
