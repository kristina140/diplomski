using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class CourseList
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CourseBase : CourseList { }

    public class CourseCreate
    {
        [Required]
        public string Name { get; set; }
    }

    public class CourseUpdate : CourseCreate
    {
        public int Id { get; set; }
    }

    public class CourseUpdateModel
    {
        public CourseUpdateModel()
        {
            Instances = new List<SemesterList>();
            Course = new CourseUpdate();
        }

        public int Id { get; set; }
        public CourseUpdate Course { get; set; }
        public List<SemesterList> Instances { get; set; }
    }

    public class CourseInstanceList
    {
        public CourseList Course { get; set; }
        public SemesterList Semester { get; set; }

        public CourseInstanceBase Value { get; set; }

        public string UserFriendly
        {
            get
            {
                return string.Format($"{Course.Name} - {Semester.UserFriendly}");
            }
        }
    }

    public class CourseInstanceUpdate
    {
        public CourseUpdate Course { get; set; }
        public SemesterUpdate Semester { get; set; }
    }

    public class CourseInstanceCreate : IValidatableObject
    {
        [Required(ErrorMessage = "Course is required!")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Semester is required!")]
        public int SemesterId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CourseId == 0)
            {
                yield return new ValidationResult("Nije odabran predmet!", new[] { "CourseId" });
            }

            if (SemesterId == 0)
            {
                yield return new ValidationResult("Nije odabran semestar!", new[] { "SemesterId" });
            }
        }
    }

    public class CourseInstanceBase
    {
        public int CourseId { get; set; }
        public int SemesterId { get; set; }
    }
}
