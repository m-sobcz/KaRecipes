using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.RecipeAggregate
{
    public class ParameterStation
    {
        public List<ParameterSingle> ParameterSingles { get; set; } = new();
        public string Name { get; set; }
    }
}
