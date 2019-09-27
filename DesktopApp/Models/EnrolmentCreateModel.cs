using CoreApp.BusinessModels;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class EnrolmentCreateModel : NotifyPropertyChangedBase
    {
        public EnrolmentCreate Enrolment { get; set; }

        public StudentBase Student { get; set; }
        public CourseList Course { get; set; }
        public SemesterList Semester { get; set; }
    }
}
