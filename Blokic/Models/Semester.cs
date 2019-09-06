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

        public virtual ICollection<CourseInstance> CourseInstance { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            var allowedMonths = new List<int> { 2, 3, 9, 10 };
            if (!allowedMonths.Contains(StartDate.Month))
            {
                errors.Add(nameof(StartDate), "Start date month invalid! Allowed months: February, March, September, October.");
            }

            if (EndDate.HasValue && StartDate >= EndDate)
            {
                errors.Add(nameof(EndDate), "End date must be after start date.");
            }

            return errors;
        }
    }
}
