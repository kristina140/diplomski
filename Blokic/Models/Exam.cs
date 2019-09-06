using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class Exam
    {
        public Exam()
        {
            StudentExam = new HashSet<StudentExam>();
        }

        public int Id { get; set; }
        public int Type { get; set; }
        public DateTime? Date { get; set; }
        public int CourseId { get; set; }
        public int SemesterId { get; set; }

        public virtual CourseInstance CourseInstance { get; set; }
        public virtual ICollection<StudentExam> StudentExam { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (Type < 1 || Type > 3)
            {
                errors.Add(nameof(Type), "Invalid exam type.");
            }

            return errors;
        }
    }
}
