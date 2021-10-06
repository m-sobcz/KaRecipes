using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.RecipeAggregate
{
    public class RawRecipe
    {
        public List<ParameterModule> ParameterModules { get; set; } = new();
        public int? VersionId { get; set; }
        public string Name { get; set; }
    }
}
