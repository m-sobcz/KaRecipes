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
        IPlcDataAccess plcDataAccess;
        public event EventHandler<string> RemovedUnknownParameter;
        static Regex stationRegex = new(@"DB.+", RegexOptions.Compiled);
        public RecipeValidator(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public async Task<RecipeData> Validate(RawRecipe sourceRecipe)
        {
            RecipeData converted = new() { Modules = sourceRecipe.ParameterModules, Name = sourceRecipe.Name, VersionId = sourceRecipe.VersionId };
            Dictionary<string, string> availableNodes = await plcDataAccess.GetAvailableNodes();
            foreach (var module in converted.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params.ToList())
                    {
                        var path = GetRawNodeIdentifier(module.Name, station.Name, parameter.Name);
                        if (availableNodes.TryGetValue(path, out string _))
                        {
                            var readVal = await plcDataAccess.ReadDataNode(path);
                            var newConvertedVal = Convert.ChangeType(parameter.Value, readVal.Value.GetType());
                            parameter.Value = newConvertedVal;
                        }
                        else
                        {
                            station.Params.Remove(parameter);
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
