using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KaRecipes.UI.ViewModels
{
    public class DatabaseTabViewModel : ViewModelBase
    {
        public DatabaseTabViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand) : base(getCommand)
        {

        }
    }
}
