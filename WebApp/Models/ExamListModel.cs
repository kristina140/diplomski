using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class ExamListModel : ExamUpdateList
    {
        public string ExamTypeDescription { get; set; }
    }

}
