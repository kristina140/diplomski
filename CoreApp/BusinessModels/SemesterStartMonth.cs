using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CoreApp.BusinessModels
{
    public enum WinterSemesterStartMonth
    {
        [Description("Rujan")]
        September = 9,
        [Description("Listopad")]
        October = 10 
    }

    public enum SummerSemesterStartMonth
    {
        [Description("Veljača")]
        February = 2,
        [Description("Ožujak")]
        March = 3
    }
}
