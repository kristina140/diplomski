using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class EnrolmentList
    {
        public int Id { get; set; }
        public StudentBase Student { get; set; }
    }

    public class EnrolmentBase
    {
        public int Id { get; set; }

        public CourseBase Course { get; set; }
        public SemesterBase Semester { get; set; }
        public StudentBase Student { get; set; }

        public DateTime? GradeDate { get; set; }
        public Grade FinalGrade { get; set; }
    }

    public class StudentCardEnrolment
    {
        public StudentCardEnrolment()
        {
            Semester = new SemesterList();
            StudentExams = new List<StudentExamBase>();
        }

        public string FinalGrade { get; set; }
        public DateTime? FinalGradeDate { get; set; }
        public SemesterList Semester { get; set; }
        public List<StudentExamBase> StudentExams { get; set; }
    }

    public class EnrolmentUpdateList
    {
        public int Id { get; set; }

        public CourseBase Course { get; set; }
        public SemesterBase Semester { get; set; }
        public StudentBase Student { get; set; }

        public EnrolmentUpdate Enrolment { get; set; }
    }

    public class EnrolmentBaseCreate
    {
        [Required(ErrorMessage = "Nije odabran student.")]
        public int StudentId { get; set; }
        [Required(ErrorMessage = "Nije odabrana instanca predmeta.")]
        public CourseInstanceBase CourseInstance { get; set; }
    }

    public class EnrolmentCreate : EnrolmentBaseCreate, IValidatableObject
    {
        public DateTime? GradeDate { get; set; }
        public Grade FinalGrade { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FinalGrade != Grade.NoGrade && !GradeDate.HasValue)
            {
                yield return new ValidationResult("Please assign date for given grade.", new[] { "GradeDate" });
            }

            if (FinalGrade == Grade.NoGrade && GradeDate.HasValue)
            {
                yield return new ValidationResult("Please assign grade for chosen date.", new[] { "FinalGrade" });
            }
        }
    }

    public class EnrolmentUpdate : EnrolmentCreate
    {
        public int Id { get; set; }
    }
}
