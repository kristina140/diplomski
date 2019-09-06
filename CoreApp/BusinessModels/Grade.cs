using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreApp.BusinessModels
{
    public enum Grade
    {
        [Description("Neocijenjen")]
        NoGrade = 0,
        [Description("Nedovoljan")]
        Insufficient = 1,
        [Description("Dovoljan")]
        Sufficient = 2,
        [Description("Dobar")]
        Good = 3,
        [Description("Vrlo dobar")]
        VeryGood = 4,
        [Description("Odlican")]
        Excellent = 5
    }

    public static class GradeConverter
    {
        public static Grade ConvertToGrade (this int? grade)
        {
            if (grade.HasValue && (grade > 5 || grade < 1))
                return Grade.NoGrade;

            return grade.HasValue ? (Grade)grade : Grade.NoGrade;
        }

        public static int? ConvertGrade(this Grade grade)
        {
            return grade == Grade.NoGrade ? null : (int?)grade;
        }

        public static int? ConvertGrade(this Grade? grade)
        {
            return grade == null || grade == Grade.NoGrade ? null : (int?)grade;
        }
    }
}