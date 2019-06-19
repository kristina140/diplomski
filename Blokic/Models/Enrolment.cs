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
    }
}
