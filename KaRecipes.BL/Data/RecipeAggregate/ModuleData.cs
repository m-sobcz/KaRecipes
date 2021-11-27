using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.RecipeAggregate
{
    public class ModuleData
    {
        public List<StationData> Stations { get; set; } = new();
        public string Name { get; set; }
    }
}
