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
        RecipeData Recipe { get; set; }
        RawRecipeData RawRecipe { get; set; }
        public string Name { get; set; }
        public ModuleData(RecipeData recipe, string name)
        {
            Recipe = recipe;
            Name = name;
        }
        public ModuleData(RawRecipeData recipe, string name)
        {
            RawRecipe = recipe;
            Name = name;
        }
        public StationData AddStation(string name) 
        {
            StationData stationData = new(this, name);
            Stations.Add(stationData);
            return stationData;
        }
    }
}
