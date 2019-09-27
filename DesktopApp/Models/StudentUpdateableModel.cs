using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class StudentUpdateableModel : UpdateableModelBase
    {
        public StudentUpdateableModel()
        {
            Student = new StudentUpdate();
        }

        private StudentUpdate _student;
        public StudentUpdate Student 
        {
            get => _student;
            set => SetProperty(ref _student, value); 
        }
    }
}
