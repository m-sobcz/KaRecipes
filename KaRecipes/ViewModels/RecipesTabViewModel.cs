using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Serialize;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<ParameterModule> ParameterModules { get; set; }
        ICommand openFileCommand;
        public event OpenFileEventHandler OpenFile;
        public delegate (string,string) OpenFileEventHandler(object sender);
        IRecipeSerializer recipeSerializer;
        public RecipesTabViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand,
            IRecipeSerializer recipeSerializer) : base(getCommand)
        {
            this.recipeSerializer = recipeSerializer;
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
                            Recipe recipe=recipeSerializer.Deserialize(OpenedFileContent);
                            ParameterModules = new(recipe.ParameterModules);
                            OnPropertyChanged(nameof(ParameterModules));
                            recipeSerializer.FillRecipeWithHeaderInfo(recipe, OpenedFileName);
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
