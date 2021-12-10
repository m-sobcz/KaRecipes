
using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.RecipeAggregate
{
    public class RawRecipeData
    {
        public List<ModuleData> Modules { get; set; } = new();
        public int? VersionId { get; set; }
        public string Name { get; set; }
    }
}
