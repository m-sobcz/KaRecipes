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
    public class RecipeChanger : IRecipeChanger
    {
        Recipe ActualRecipe { get; set; }
        readonly IPlcDataAccess plcDataAccess;
        public event EventHandler<string> WriteToNodeFailed;
        public RecipeChanger(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public void Initialize(Recipe recipeTemplate)
        {
            ActualRecipe = recipeTemplate;
        }
        public async Task WriteToPlc(Recipe recipe)
        {
            foreach (var module in recipe.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    {
                        string path = GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        bool writingOk = await plcDataAccess.WriteParameter(path, parameter.Value);
                        if (writingOk == false) OnWriteToNodeFailed(path);
                    }
                }
            }
        }
        public async Task<Recipe> ReadFromPlc()
        {
            if (ActualRecipe is null) throw new InvalidOperationException("Call Initialize with reference Recipe before reading from PLC");
            Recipe recipe = DeepCopier.Copy(ActualRecipe);
            foreach (var module in recipe.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    { 
                        string path = GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        var node = await plcDataAccess.ReadParameter(path);
                        parameter.Value = node.Value;
                    }
                }
            }
            return recipe;
        }
        public async Task<Recipe> Subscribe()
        {
            if (ActualRecipe is null) throw new InvalidOperationException("Call Initialize with reference Recipe before reading from PLC");
            List<string> paths = new();
            Recipe recipe = DeepCopier.Copy(ActualRecipe);
            foreach (var module in recipe.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    {
                        string path = GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        paths.Add(path);
                    }
                }
            }
            await plcDataAccess.CreateSubscriptionsWithInterval(paths, 1000);
            plcDataAccess.OpcDataReceived += PlcDataAccess_OpcDataReceived;
            return recipe;
        }

        private void PlcDataAccess_OpcDataReceived(object sender, PlcDataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnWriteToNodeFailed(string path)
        {
            WriteToNodeFailed?.Invoke(this, path);
        }
        string GetNodeIdentifier(string module, string station, string parameter)
        {
            string path = $"{plcDataAccess.PlcAccessPrefix}.{module}.{station}.{parameter}";
            return path;
        }
    }
}
