using DeepCopy;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public class RecipeVerificator
    {
        IPlcDataAccess plcDataAccess;
        public event EventHandler<string> RemovedUnknownParameter;
        public RecipeVerificator(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public async Task<Recipe> CheckAndSetParameterTypes(Recipe sourceRecipe) 
        {
            Recipe converted = DeepCopier.Copy(sourceRecipe);
            Dictionary<string, string> availableNodes = plcDataAccess.GetAvailableNodes();
            foreach (var module in converted.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    {
                        var path = PlcNode.GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        if (availableNodes.TryGetValue(path, out string _)) 
                        {
                            string nodeId = PlcNode.GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                            var readVal=await plcDataAccess.ReadParameter(nodeId);
                            var newConvertedVal=Convert.ChangeType(parameter.Value, readVal.Value.GetType());
                            parameter.Value = newConvertedVal;
                        }
                        else
                        {
                            station.ParameterSingles.Remove(parameter);
                            OnRemovedUnknownParameter(path);
                        }
                        ;
                    }
                }
            }
            return converted;
        }
        void OnRemovedUnknownParameter(string recipeId) 
        {
            RemovedUnknownParameter?.Invoke(this, recipeId);
        }
    }
}
