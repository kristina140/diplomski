
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Utility
{
    public class ConfirmationButton : Button
    {
        public string Question { get; set; }
        public string Caption { get; set; }

        protected override void OnClick()
        {
            if (string.IsNullOrWhiteSpace(Question))
            {
                base.OnClick();
                return;
            }

            var messageBoxResult = MessageBox.Show(Question, Caption, MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (messageBoxResult == MessageBoxResult.Yes)
                base.OnClick();
        }
    }

}
