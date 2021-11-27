using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public class RawRecipe
    {
        public List<ModuleData> ParameterModules { get; set; } = new();
        public int? VersionId { get; set; }
        public string Name { get; set; }
    }
}
