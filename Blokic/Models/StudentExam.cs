using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class StudentExam
    {
        public StudentExam()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public int EnrolmentId { get; set; }
        public int ExamId { get; set; }
        public bool Participated { get; set; }
        public double? Score { get; set; }
        public int? Grade { get; set; }
        public string Description { get; set; }

        public virtual Enrolment Enrolment { get; set; }
        public virtual Exam Exam { get; set; }

        public IDictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (Grade.HasValue && (Grade < 1 || Grade > 5))
            {
                errors.Add(nameof(Grade), "Invalid grade.");
            }

            return errors;
        }
    }
}
