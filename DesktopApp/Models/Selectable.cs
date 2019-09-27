using DesktopApp.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace DesktopApp.Models
{
    public class Selectable<T> : NotifyPropertyChangedBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public T Item { get; set; }
    }
}
