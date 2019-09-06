using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class StudentExamsCreateModel : StudentExamsCreate
    {
        public ExamListModel ExamList { get; set; }
    }
}
