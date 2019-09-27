using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopApp.Models
{
    public abstract class UpdateableModelBase : NotifyPropertyChangedBase
    {
        private bool _inEditMode;
        public bool InEditMode
        {
            get => _inEditMode;
            set => SetProperty(ref _inEditMode, value);
        }
    }
}
