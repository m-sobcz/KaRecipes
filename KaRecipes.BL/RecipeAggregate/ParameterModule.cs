using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.RecipeAggregate
{
    public class ParameterModule
    {
        public List<ParameterStation> ParameterStations { get; set; } = new();
        public string Name { get; set; }
    }
}
