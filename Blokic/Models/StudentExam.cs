using System;
using System.Collections.Generic;

namespace Blokic.Models
{
    public partial class StudentExam
    {
        public int EnrolmentId { get; set; }
        public int ExamId { get; set; }
        public bool Participated { get; set; }
        public double? Score { get; set; }
        public int? Grade { get; set; }
        public string Description { get; set; }

        public virtual Enrolment Enrolment { get; set; }
        public virtual Exam Exam { get; set; }
    }
}
