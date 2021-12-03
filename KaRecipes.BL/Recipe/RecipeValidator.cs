using DeepCopy;
using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public class RecipeValidator : IRecipeValidator
    {
        public event EventHandler<string> UnknownParameterFound;
        public event EventHandler<string> UnsetParameterFound;
        static Regex stationRegex = new(@"DB.+", RegexOptions.Compiled);
        readonly string pathPrefix = "KaRecipes";
        public RecipeData Validate(RawRecipe sourceRecipe, Dictionary<string, Type> recipeNodes)
        {
            RecipeData converted = new() { Modules = sourceRecipe.ParameterModules, Name = sourceRecipe.Name, VersionId = sourceRecipe.VersionId };
            foreach (var module in converted.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params.ToList())
                    {
                        var path = GetRawNodeIdentifier(module.Name, station.Name, parameter.Name);
                        if (recipeNodes.Remove(path, out Type type))
                        {
                            var newConvertedVal = Convert.ChangeType(parameter.Value, type);
                            parameter.Value = newConvertedVal;
                        }
                        else
                        {
                            station.Params.Remove(parameter);
                            OnUnknownParameterFound(path);
                        }
                    }             
                }
            }
            foreach (var node in recipeNodes.Keys)
            {
                OnUnsetParameterFound(node);
            }
            return converted;
        }
        string GetRawNodeIdentifier(string module, string station, string parameter)
        {
            string stationName = stationRegex.Match(station).Value;
            string path = $"{pathPrefix}.{module}.{stationName}.{parameter}";
            return path;
        }
        void OnUnknownParameterFound(string recipeId)
        {
            UnknownParameterFound?.Invoke(this, recipeId);
        }
        void OnUnsetParameterFound(string recipeId)
        {
            UnsetParameterFound?.Invoke(this, recipeId);
        }
    }
}
