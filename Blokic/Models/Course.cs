using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class Course
    {
        public Course()
        {
            CourseInstance = new HashSet<CourseInstance>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<CourseInstance> CourseInstance { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add(nameof(Name), "Course name is required.");
            }

            return errors;
        }
    }
}
