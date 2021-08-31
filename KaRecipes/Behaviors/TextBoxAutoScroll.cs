using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace KaRecipes.UI.Behaviors
{
    public class TextBoxAutoScroll : Behavior<TextBox>
    {
        private TextBox textBox;
        protected override void OnAttached()
        {
            base.OnAttached();
            textBox = base.AssociatedObject;
            textBox.TextChanged += TextBox_TextChanged;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox.ScrollToEnd();
        }
    }

}
