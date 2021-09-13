using KaRecipes.BL.RecipeAggregate;
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
        public ObservableCollection<ParameterModule> ParameterModules { get; set; }

        public RecipeUI(Recipe recipe)
        {
            ParameterModules = new(recipe.ParameterModules);
        }

    }
}
