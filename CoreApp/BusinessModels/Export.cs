using System;
using System.Collections.Generic;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class StudentExamsExport
    {
        public List<ExamExport> Exams { get; set; }
        public List<EnrolmentExport> Students { get; set; }
        public Dictionary<Tuple<int, int>, StudentExamExport> StudentExams { get; set; }

        public string Course { get; set; }
        public string Semester { get; set; }
    }

    public class ExamExport
    {
        public int ExamId { get; set; }
        public DateTime? ExamDate { get; set; }
        public ExamType ExamType { get; set; }
        public SemesterList Semestar { get; set; }
    }

    public class EnrolmentExport
    {
        public StudentBase Student { get; set; }
        public DateTime? FinalGradeDate { get; set; }
        public int? FinalGrade { get; set; }

        //semestar
    }

    public class StudentExamExport
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public Guid StudentExamId { get; set; }

        public bool Participated { get; set; }
        public double? Score { get; set; }
        public int? Grade { get; set; }
    }
}
