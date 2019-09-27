using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopApp.ViewModels
{
    public abstract class ViewModelBase: NotifyPropertyChangedBase
    {
        public ViewModelBase()
        {

        }

        private List<string> _validationErrors;
        public List<string> ValidationErrors
        {
            get => _validationErrors;
            set => SetProperty(ref _validationErrors, value);
        }

    }
}
