using KaRecipes.BL.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.UI.Models.RecipeModel
{
    class RecipeUI
    {
        public ObservableCollection<ParameterModule> parameterModules;

        public RecipeUI(Recipe recipe)
        {
            parameterModules = new(recipe.ParameterModules);
        }

    }
}
