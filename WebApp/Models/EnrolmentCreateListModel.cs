using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class EnrolmentCreateListModel
    {
        public EnrolmentCreate Enrolment { get; set; }

        public StudentBase Student { get; set; }
        public CourseList Course { get; set; }
        public SemesterList Semester { get; set; }
    }
}
