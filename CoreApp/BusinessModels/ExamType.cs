using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CoreApp.BusinessModels
{
    public enum ExamType
    {
        [Description("Nedefiniran tip")]
        Undefined = 0,
        [Description("Pismeni")]
        Written = 1,
        [Description("Usmeni")]
        Oral = 2,
        [Description("Kolokvij")]
        Midterm = 3
    }

    public static class ExamTypeConverter
    {
        public static ExamType ConvertToExamType(this int examType)
        {
            return examType > 3 || examType < 1 ? ExamType.Undefined : (ExamType)examType;
        }

        public static int ConvertExamType(this ExamType examType)
        {
            return examType == ExamType.Undefined ? 0 : (int)examType;
        }
    }
}
