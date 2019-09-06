using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreApp.BusinessModels
{
    public class ExistingStudentOralExam
    {
        public Guid Id { get; set; }
        public string Exam { get; set; }
        public string Student { get; set; }
    }

    public class OralExamCreate
    {
        [Required(ErrorMessage = "Predmet je obavezan!")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Semestar je obavezan!")]
        public int SemesterId { get; set; }
        [Required(ErrorMessage = "Student je obavezan!")]
        public int EnrolmentId { get; set; }

        /*
       if null, then create new exam
           */
        public int? ExamId { get; set; }
        public OralExamCreateExam Exam { get; set; }

        public OralExamDateTime ExistingExamDateTime { get; set; }
    }

    public class OralExamList
    {
        public ExamBase Exam { get; set; }
        public EnrolmentList Enrolment { get; set; }

        public string Course { get; set; }
        public int CourseId { get; set; }

        public OralExamUpdate StudentOralExamUpdate { get; set; }
    }

    public class OralExamUpdate
    {
        public int EnrolmentId { get; set; }
        public int ExamId { get; set; }
        public Guid Id { get; set; }

        public bool Participated { get; set; }
        public double? Score { get; set; }
        public Grade? Grade { get; set; }
        public string Description { get; set; }

        public DateTime? FinalGradeDate { get; set; }
        public Grade? FinalGrade { get; set; }

    }

    /*
     Type is implicity Oral(int = 2)
         */
    public class OralExamCreateExam
    {
        [Required(ErrorMessage = "Datum ispita je obavezan!")]
        public DateTime Date { get; set; }
        public DateTime? Time { get; set; }

        [Required(ErrorMessage = "Instanca predmeta je obavezna!")]
        public CourseInstanceBase CourseInstance { get; set; }
    }

    public class OralExamDateTime
    {
        [Required(ErrorMessage = "Datum ispita je obavezan!")]
        public DateTime Date { get; set; }
        public DateTime? Time { get; set; }
    }
}
