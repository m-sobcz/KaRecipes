using System;
using System.Windows.Input;
using System.ComponentModel;

namespace KaRecipes.UI.ViewModels
{
    public class ObservedObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(params string[] propertyName)
        {
            if (PropertyChanged != null)
            {
                foreach (string s in propertyName)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(s));
                }
            }
        }
    }
}