using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KaRecipes.UI.ViewModels
{
    public class DiagnosticsTabViewModel : ViewModelBase
    {
        public DiagnosticsTabViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand) : base(getCommand)
        {

        }
    }
}
