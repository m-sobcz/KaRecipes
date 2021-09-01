using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace KaRecipes.UI.ViewModels
{
    public class ViewModelBase : ObservedObject
    {
        protected Func<Action<object>, Func<object, bool>, ICommand> getCommand;
        public event ShowInfoEventHandler ShowInformation;
        public delegate void ShowInfoEventHandler(object sender, string text, string caption);
        public ViewModelBase(Func<Action<object>, Func<object, bool>, ICommand> getCommand)
        {
            this.getCommand = getCommand;
        }
        public void ShowInfo(string text, string caption = "---")
        {
            ShowInformation?.Invoke(this, text, caption);
        }
    }
}
