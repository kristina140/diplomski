using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class ExamBase
    {
        public int Id { get; set; }
        public string UserFriendly { get; set; }
    }

    public class ExamList
    {
        public int Id { get; set; }
        public int CourseId { get; set; }

        public DateTime? Date { get; set; }

        public string UserFriendly { get; set; }
    }

    public class ExamUpdateList
    {
        public int Id { get; set; }
        public ExamType Type { get; set; }

        public ExamUpdate Exam { get; set; }

        public CourseList Course { get; set; }
        public SemesterList Semester { get; set; }
    }

    public class ExamCreate
    {
        public ExamCreate()
        {
            EnrolmentIds = new List<int>();
        }

        [Required(ErrorMessage = "Tip ispita je obavezan!")]
        public ExamType Type { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }

        [Required(ErrorMessage = "Instanca predmeta je obavezna!")]
        public CourseInstanceBase CourseInstance { get; set; }

        public List<int> EnrolmentIds { get; set; }
    }

    public class ExamUpdate
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? Time { get; set; }
    }
}
