using CoreApp.BusinessModels;
using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class CourseInstanceListModel : NotifyPropertyChangedBase
    {
        public CourseInstanceListModel()
        {
            CourseInstance = new CourseInstanceList();
        }

        private CourseInstanceList _courseInstance;
        public CourseInstanceList CourseInstance
        {
            get => _courseInstance;
            set => SetProperty(ref _courseInstance, value);
        }
    }
}
