using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class ExamUpdateableModel : UpdateableModelBase
    {
        public string ExamTypeDescription { get; set; }

        private ExamUpdateList _exam;
        public ExamUpdateList Exam
        {
            get => _exam;
            set => SetProperty(ref _exam, value);
        }
    }
}
