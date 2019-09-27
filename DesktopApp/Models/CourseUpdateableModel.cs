using CoreApp.BusinessModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopApp.Models
{
    public class CourseUpdateableModel : UpdateableModelBase
    {
        public CourseUpdateableModel()
        {
            Instances = new List<SemesterList>();
        }


        private int _id;
        public int Id 
        { 
            get => _id; 
            set => SetProperty(ref _id, value); 
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }


        public List<SemesterList> Instances { get; set; }


    }
}
