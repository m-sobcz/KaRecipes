using DeepCopy;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public class RecipeValidator : IRecipeValidator
    {
        IPlcDataAccess plcDataAccess;
        public event EventHandler<string> RemovedUnknownParameter;
        static Regex stationRegex = new(@"DB.+", RegexOptions.Compiled);
        public RecipeValidator(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public async Task<Recipe> Validate(RawRecipe sourceRecipe)
        {
            Recipe converted = new() { ParameterModules = sourceRecipe.ParameterModules, Name = sourceRecipe.Name, VersionId = sourceRecipe.VersionId };
            Dictionary<string, string> availableNodes = plcDataAccess.GetAvailableNodes();
            foreach (var module in converted.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles.ToList())
                    {
                        var path = GetRawNodeIdentifier(module.Name, station.Name, parameter.Name);
                        if (availableNodes.TryGetValue(path, out string _))
                        {
                            var readVal = await plcDataAccess.ReadParameter(path);
                            var newConvertedVal = Convert.ChangeType(parameter.Value, readVal.Value.GetType());
                            parameter.Value = newConvertedVal;
                        }
                        else
                        {
                            station.ParameterSingles.Remove(parameter);
                            OnRemovedUnknownParameter(path);
                        }
                    }
                }
            }
            return converted;
        }
        string GetRawNodeIdentifier(string module, string station, string parameter)
        {
            string stationName = stationRegex.Match(station).Value;
            string path = $"{plcDataAccess.PlcAccessPrefix}.{module}.{stationName}.{parameter}";
            return path;
        }
        void OnRemovedUnknownParameter(string recipeId)
        {
            RemovedUnknownParameter?.Invoke(this, recipeId);
        }
    }
}
