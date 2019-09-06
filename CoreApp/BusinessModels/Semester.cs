using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class SemesterList
    {
        public int Id { get; set; }
        public string AcademicYear
        {
            get
            {
                return IsWinter ? 
                    string.Format(($"{StartDate.Year}/{StartDate.Year + 1}")) : 
                    string.Format(($"{StartDate.Year - 1}/{StartDate.Year}"));
            }
        }
        public bool IsWinter { get; set; }
        public DateTime StartDate { get; set; }
        public string UserFriendly
        {
            get
            {
                return IsWinter ?
                   string.Format($"zimski {StartDate.Year}/{StartDate.Year + 1}") :
                   string.Format($"ljetni {StartDate.Year - 1}/{StartDate.Year}");

            }
        }
    }

    public class SemesterBase : SemesterList
    {
        public DateTime? EndDate { get; set; }
    }

    public class SemesterCreate : IValidatableObject
    {
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsWinter { get; set; }
        public string AcademicYear { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var allowedMonths = new List<int> { 2, 3, 9, 10 };

            if (!allowedMonths.Contains(StartDate.Month))
            {
                yield return new ValidationResult("Start date month invalid! Allowed months: February, March, September, October.", new[] { "StartDate" });
            }

            if (EndDate.HasValue && StartDate >= EndDate)
            {
                yield return new ValidationResult("End date must be after start date!", new[] { "StartDate, EndDate" });
            }
        }
    }

    public class SemesterUpdate : SemesterCreate
    {
        public int Id { get; set; }
    }

}
