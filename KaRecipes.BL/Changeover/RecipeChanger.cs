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
        public Recipe ActualRecipe { get; set; }

        public int PublishingInterval => 1000;

        readonly IPlcDataAccess plcDataAccess;
        public event EventHandler<Recipe> ActualRecipeChanged;
        Regex regex = new(@"([^.]+)\.([^.]+)\.([^.]+)\.([^.]+)", RegexOptions.Compiled);
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
            List<DataNode> nodesToWrite = new();
            foreach (var module in recipe.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    {
                        string path = GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        nodesToWrite.Add(new DataNode() { NodeId = path, Value = parameter.Value });
                    }
                }
            }
            await plcDataAccess.WriteDataNodes(nodesToWrite);
            ActualRecipe = recipe;
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
                        var node = await plcDataAccess.ReadDataNode(path);
                        parameter.Value = node.Value;
                    }
                }
            }
            ActualRecipe = recipe;
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
            await plcDataAccess.CreateSubscriptionsWithInterval(paths, PublishingInterval, this);
            return recipe;
        }

        string GetNodeIdentifier(string module, string station, string parameter)
        {
            string path = $"{plcDataAccess.PlcAccessPrefix}.{module}.{station}.{parameter}";
            return path;
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            string name = subject.Name;
            var match = regex.Match(name);
            var module = ActualRecipe.ParameterModules.Where(x => x.Name == match.Groups[2].Value).FirstOrDefault();
            var station=module.ParameterStations.Where(x => x.Name == match.Groups[3].Value).FirstOrDefault();
            var parameter = station.ParameterSingles.Where(x => x.Name == match.Groups[4].Value).FirstOrDefault();
            if(parameter.Value != subject.Value) 
            {
                parameter.Value = subject.Value;
                ActualRecipeChanged?.Invoke(this, ActualRecipe);
            }    
        }
    }
}
