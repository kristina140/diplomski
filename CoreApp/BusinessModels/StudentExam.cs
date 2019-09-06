using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class StudentExamBase
    {
        public bool Participated { get; set; }
        public double? Score { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }

        public string ExamType { get; set; }
        public DateTime? ExamDate { get; set; }
    }

    public class StudentExamBaseList : StudentExamBase
    {
        public string Semester { get; set; }
    }

    public class StudentExamList : StudentExamBaseList
    {
        public string Course { get; set; }
    }

    public class StudentExamsCreate
    {
        public List<int> EnrolmentIds { get; set; }
        public int ExamId { get; set; }
    }

    public class StudentExamCreate
    {
        public int EnrolmentId { get; set; }
        public int ExamId { get; set; }
    }

    public class StudentExamUpdate : StudentExamCreate
    {
        public bool Participated { get; set; }
        public double? Score { get; set; }
        public Grade Grade { get; set; }
        public string Description { get; set; }
    }

    public class StudentExamUpdateList
    {
        public Guid StudentExamId { get; set; }
        public StudentExamUpdate StudentExam { get; set; }
        public StudentBase Student { get; set; }
    }
}
