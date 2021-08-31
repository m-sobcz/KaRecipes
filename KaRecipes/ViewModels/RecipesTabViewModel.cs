using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KaRecipes.UI.ViewModels
{
    public class RecipesTabViewModel : ViewModelBase
    {
        public string OpenedFileName { get; set; }
        ICommand openFileCommand;
        public event OpenFileEventHandler OpenFile;
        public delegate string OpenFileEventHandler(object sender);
        public ICommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                {
                    openFileCommand = new RelayCommand(
                        o =>
                        {
                            OpenedFileName=OpenFile?.Invoke(this);
                            OnPropertyChanged(nameof(OpenedFileName));
                        },
                        o => true
                        );
                }
                return openFileCommand;
            }
        }
    }
}
