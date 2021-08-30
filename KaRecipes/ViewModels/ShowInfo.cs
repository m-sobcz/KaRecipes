using System;
using System.Collections.Generic;
using System.Text;

namespace KaRecipes.UI.ViewModels
{
    public class ShowInfo
    {
        public event ShowInfoEventHandler ShowInformation;
        public delegate void ShowInfoEventHandler(object sender, string text, string caption);
        public void Show(string text, string caption = "---")
        {
            ShowInformation?.Invoke(this, text, caption);
        }
    }
}
