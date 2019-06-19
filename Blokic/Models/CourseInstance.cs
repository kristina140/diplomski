using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class CourseInstance
    {
        public CourseInstance()
        {
            Enrolment = new HashSet<Enrolment>();
            Exam = new HashSet<Exam>();
        }

        public int CourseId { get; set; }
        public int SemesterId { get; set; }

        public virtual Course Course { get; set; }
        public virtual Semester Semester { get; set; }
        public virtual ICollection<Enrolment> Enrolment { get; set; }
        public virtual ICollection<Exam> Exam { get; set; }
    }
}
