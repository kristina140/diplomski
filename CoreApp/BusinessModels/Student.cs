using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class StudentList
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }

    public class StudentBase : StudentList
    {
        public string Jmbag { get; set; }
        public string IndexNmb { get; set; }

        public string UserFriendly
        {
            get
            {
                return string.Format($"{Firstname} {Lastname} ({Jmbag}) {(string.IsNullOrEmpty(IndexNmb) ? string.Empty : IndexNmb)}");
            }
        }
    }

    public class StudentCreate : IValidatableObject
    {
        [Required(ErrorMessage = "Firstname is required!")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Lastname is required!")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "JMBAG is required!")]
        public string Jmbag { get; set; }
        public string IndexNmb { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Jmbag) || Jmbag.Length != 10 || !Jmbag.All(_ => char.IsDigit(_)))
            {
                yield return new ValidationResult("JMBAG se mora sastojati od 10 znamenki", new[] { "Jmbag" });

            }
        }
    }

    public class StudentUpdate : StudentCreate
    {
        public int Id { get; set; }
    }

    public class StudentCard
    {
        public StudentCard()
        {
            Student = new StudentBase();
            CourseEnrolments = new List<StudentCardCourseEnrolment>();
        }

        public StudentBase Student { get; set; }

        public List<StudentCardCourseEnrolment> CourseEnrolments { get; set; }
    }

    public class StudentCardCourseEnrolment
    {
        public StudentCardCourseEnrolment()
        {
            Course = new CourseList();
            Enrolments = new List<StudentCardEnrolment>();
        }

        public CourseList Course { get; set; }
        public List<StudentCardEnrolment> Enrolments { get; set; }
    }
}
