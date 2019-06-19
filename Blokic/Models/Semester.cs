using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class Semester
    {
        public Semester()
        {
            CourseInstance = new HashSet<CourseInstance>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsWinter { get; set; }
        public string AcademicYear { get; set; }

        public virtual ICollection<CourseInstance> CourseInstance { get; set; }
    }
}
