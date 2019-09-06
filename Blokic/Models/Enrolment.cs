using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class Enrolment
    {
        public Enrolment()
        {
            StudentExam = new HashSet<StudentExam>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public int SemesterId { get; set; }
        public int StudentId { get; set; }
        public DateTime? GradeDate { get; set; }
        public int? FinalGrade { get; set; }

        public virtual CourseInstance CourseInstance { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<StudentExam> StudentExam { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (FinalGrade.HasValue && (FinalGrade < 1 || FinalGrade > 5))
            {
                errors.Add(nameof(FinalGrade), "Invalid grade.");
            }

            if (FinalGrade.HasValue && !GradeDate.HasValue)
            {
                errors.Add(nameof(GradeDate), "Date for given grade is not assigned.");
            }

            if (!FinalGrade.HasValue && GradeDate.HasValue)
            {
                errors.Add(nameof(FinalGrade), "Grade is not assigned.");
            }

            return errors;
        }
    }
}
