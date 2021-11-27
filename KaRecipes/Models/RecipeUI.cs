using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.UI.Models
{
    public class RecipeUI
    {
        public ObservableCollection<ModuleData> ParameterModules { get; set; }

        public RecipeUI(RecipeData recipe)
        {
            ParameterModules = new(recipe.Modules);
        }

    }
}
