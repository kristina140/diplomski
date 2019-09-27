using CoreApp.BusinessModels;
using CoreApp.IServices;
using DesktopApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Utility
{
    public static class Mapper
    {
        #region Courses
        public static CourseUpdateableModel MapCourseUpdateableModel(CourseUpdateModel source)
        {
            return new CourseUpdateableModel
            {
                Id = source.Id,
                InEditMode = false,
                Name = source.Course.Name,
                Instances = source.Instances
            };
        }

        public static CourseUpdateableModel MapCourseUpdateableModel(this CourseUpdateableModel dest, CourseUpdate source)
        {
            dest.Name = source.Name;
            return dest;
        }

        public static CourseUpdateModel MapCourseUpdateModel(CourseUpdateableModel source)
        {
            return new CourseUpdateModel
            {
                Id = source.Id,
                Course = new CourseUpdate
                {
                    Id = source.Id,
                    Name = source.Name
                },
                Instances = source.Instances
            };
        }

        public static CourseUpdate MapCourseUpdate(CourseUpdateableModel source)
        {
            return new CourseUpdate
            {
                Id = source.Id,
                Name = source.Name
            };
        }
        #endregion

        #region Students
        public static StudentUpdateableModel MapStudentUpdateableModel(StudentUpdate source)
        {
            return new StudentUpdateableModel
            {
                Student = new StudentUpdate
                {
                    Id = source.Id,
                    Firstname = source.Firstname,
                    IndexNmb = source.IndexNmb,
                    Jmbag = source.Jmbag,
                    Lastname = source.Lastname
                }
            };
        }

        public static StudentUpdate MapStudentUpdate(StudentUpdateableModel source)
        {
            return new StudentUpdate
            {
                Id = source.Student.Id,
                Firstname = source.Student.Firstname,
                IndexNmb = source.Student.IndexNmb,
                Jmbag = source.Student.Jmbag,
                Lastname = source.Student.Lastname
            };
        }

        public static StudentUpdateableModel MapStudentUpdateableModel(this StudentUpdateableModel dest, StudentUpdate source)
        {
            dest.Student.Id = source.Id;
            dest.Student.Firstname = source.Firstname;
            dest.Student.IndexNmb = source.IndexNmb;
            dest.Student.Jmbag = source.Jmbag;
            dest.Student.Lastname = source.Lastname;

            return dest;
        }
        #endregion

        #region Semesters
        public static SemesterUpdateableModel MapSemesterUpdateableModel(SemesterUpdate source)
        {
            return new SemesterUpdateableModel
            {
                EndDate = source.EndDate,
                Id = source.Id,
                StartDate = source.StartDate
            };
        }

        public static SemesterUpdateableModel MapSemesterUpdateableModel(this SemesterUpdateableModel dest, SemesterUpdate source)
        {
            dest.EndDate = source.EndDate;
            dest.Id = source.Id;
            dest.StartDate = source.StartDate;
            
            return dest;
        }

        public static SemesterUpdate MapSemesterUpdate(SemesterUpdateableModel source)
        {
            return new SemesterUpdate
            {
                Id = source.Id,
                AcademicYear = source.AcademicYear,
                EndDate = source.EndDate,
                IsWinter = source.IsWinter,
                StartDate = source.StartDate
            };
        }

        public static SemesterCreate MapSemesterCreate (SemesterCreateModel source)
        {
            return new SemesterCreate
            {
                StartDate = source.StartDate,
                AcademicYear = source.AcademicYear,
                EndDate = source.EndDate,
                IsWinter = source.IsWinter
            };
        }
        #endregion

        #region Course Instances
        public static CourseInstanceListModel MapCourseInstanceListModel(CourseInstanceList source)
        {
            return new CourseInstanceListModel
            {
                CourseInstance = source
            };
        }

        #endregion

        #region Enrolments
        public static EnrolmentUpdateableModel MapEnrolmentUpdateableModel(EnrolmentUpdateList source)
        {
            return new EnrolmentUpdateableModel
            {
                FinalGrade = source.Enrolment.FinalGrade,
                Enrolment = new EnrolmentUpdateList
                {
                    Id = source.Enrolment.Id,
                    Enrolment = new EnrolmentUpdate
                    {
                        Id = source.Enrolment.Id,
                        CourseInstance = new CourseInstanceBase
                        {
                            CourseId = source.Enrolment.CourseInstance.CourseId,
                            SemesterId = source.Enrolment.CourseInstance.SemesterId
                        },
                        FinalGrade = source.Enrolment.FinalGrade,
                        GradeDate = source.Enrolment.GradeDate,
                        StudentId = source.Enrolment.StudentId
                    },
                    Course = source.Course,
                    Semester = source.Semester,
                    Student = source.Student
                }
            };
        }

        public static EnrolmentUpdateableModel MapEnrolmentUpdateableModel(this EnrolmentUpdateableModel dest, EnrolmentUpdateList source)
        {
            dest.Enrolment.Id = source.Enrolment.Id;
            dest.Enrolment.Enrolment.Id = source.Enrolment.Id;
            dest.Enrolment.Enrolment.CourseInstance.CourseId = source.Enrolment.CourseInstance.CourseId;
            dest.Enrolment.Enrolment.CourseInstance.SemesterId = source.Enrolment.CourseInstance.SemesterId;
            dest.Enrolment.Enrolment.FinalGrade = source.Enrolment.FinalGrade;
            dest.Enrolment.Enrolment.GradeDate = source.Enrolment.GradeDate;
            dest.Enrolment.Enrolment.StudentId = source.Enrolment.StudentId;
            dest.Enrolment.Course = source.Course;
            dest.Enrolment.Semester = source.Semester;
            dest.Enrolment.Student = source.Student;
            dest.FinalGrade = source.Enrolment.FinalGrade;

            return dest;
        }
        
        public static EnrolmentUpdate MapEnrolmentUpdate (EnrolmentUpdateableModel source)
        {
            return new EnrolmentUpdate
            {
                Id = source.Enrolment.Id,
                CourseInstance = source.Enrolment.Enrolment.CourseInstance,
                FinalGrade = source.FinalGrade,
                GradeDate = source.Enrolment.Enrolment.GradeDate,
                StudentId = source.Enrolment.Student.Id
            };
        }

        public static EnrolmentCreateModel MapEnrolmentCreateModel(Selectable<StudentBase> source1, Selectable<CourseInstanceList> source2)
        {
            return new EnrolmentCreateModel
            {
                Student = new StudentBase
                {
                    Firstname = source1.Item.Firstname,
                    Id = source1.Item.Id,
                    IndexNmb = source1.Item.IndexNmb,
                    Jmbag = source1.Item.Jmbag,
                    Lastname = source1.Item.Lastname
                },
                Course = new CourseList 
                { 
                    Id = source2.Item.Course.Id,
                    Name = source2.Item.Course.Name
                } ,
                Semester = new SemesterList 
                { 
                    Id = source2.Item.Semester.Id,
                    StartDate = source2.Item.Semester.StartDate,
                    IsWinter = source2.Item.Semester.IsWinter
                } ,
                Enrolment = new EnrolmentCreate
                {
                    StudentId = source1.Item.Id,
                    CourseInstance = new CourseInstanceBase
                    {
                        CourseId = source2.Item.Course.Id,
                        SemesterId = source2.Item.Semester.Id
                    }
                }
            };
        }
        #endregion

        #region Exams
        public static ExamUpdateableModel MapExamUpdateableModel(ExamUpdateList source)
        {
            return new ExamUpdateableModel
            {
                Exam = new ExamUpdateList
                {
                    Exam = new ExamUpdate 
                    { 
                        Id = source.Exam.Id,
                        Date = source.Exam.Date,
                        Time = source.Exam.Time
                    },
                    Course = source.Course,
                    Id = source.Id,
                    Semester = source.Semester
                },
                ExamTypeDescription = source.Type.GetEnumDescription()
            };
        }
        #endregion

    }

    public static class Mapper<T>
    {
        public static Selectable<T> MapSelectable(T source)
        {
            return new Selectable<T>
            {
                IsSelected = false,
                Item = source
            };
        }

        public static IEnumerable<Selectable<T>> MapSelectable(List<T> source)
        {
            if (source == null)
                yield return null;
            else
            {
                foreach (var item in source)
                {
                    yield return MapSelectable(item);
                }
            }
        }
    }
}
