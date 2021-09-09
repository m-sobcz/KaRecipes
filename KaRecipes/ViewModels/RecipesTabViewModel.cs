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
        public string OpenedFileContent { get; set; }
        ICommand openFileCommand;
        public event OpenFileEventHandler OpenFile;
        public delegate (string,string) OpenFileEventHandler(object sender);
        public RecipesTabViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand) : base(getCommand)
        {

        }
        public ICommand OpenFileCommand
        {
            get
            {
                if (openFileCommand == null)
                {
                    openFileCommand = getCommand(
                        o =>
                        {
                            var result =OpenFile?.Invoke(this);
                            (OpenedFileName, OpenedFileContent) = result.Value;
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
