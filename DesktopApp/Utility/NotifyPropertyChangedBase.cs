using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DesktopApp.Utility
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        protected Dictionary<string, List<string>> DependencyMap;

        public NotifyPropertyChangedBase()
        {
            DependencyMap = new Dictionary<string, List<string>>();

            foreach (var property in GetType().GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DependsOnPropertyAttribute), true);

                foreach (DependsOnPropertyAttribute dependsAttr in attributes)
                {
                    if (dependsAttr == null)
                        continue;

                    var dependence = dependsAttr.Dependence;
                    if (!DependencyMap.ContainsKey(dependence))
                        DependencyMap.Add(dependence, new List<string>());
                    DependencyMap[dependence].Add(property.Name);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                if (PropertyChanged != null && DependencyMap.ContainsKey(propertyName))
                {
                    foreach (var dependentProperty in DependencyMap[propertyName])
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dependentProperty));
                    }
                }
                return true;
            }
            return false;
        }
    }
}
