using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Recipe;
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
        public ObservableCollection<ModuleData> Modules { get; set; }
        ICommand openFileCommand;
        public event OpenFileEventHandler OpenFile;
        public delegate (string,string) OpenFileEventHandler(object sender);
        IRawRecipeSerializer recipeSerializer;
        public RecipesTabViewModel(Func<Action<object>, Func<object, bool>, ICommand> getCommand,
            IRawRecipeSerializer recipeSerializer) : base(getCommand)
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
                            RawRecipe recipe=recipeSerializer.Deserialize(OpenedFileContent);
                            Modules = new(recipe.ParameterModules);
                            OnPropertyChanged(nameof(Modules));
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
