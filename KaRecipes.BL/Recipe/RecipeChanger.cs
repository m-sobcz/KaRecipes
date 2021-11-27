using DeepCopy;
using KaRecipes.BL.Data;
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
    public class RecipeChanger : IRecipeChanger
    {
        public RecipeData ActualRecipe { get; set; }

        public int PublishingInterval => 1000;

        readonly IPlcDataAccess plcDataAccess;
        public event EventHandler<RecipeData> ActualRecipeChanged;
        Regex regex = new(@"([^.]+)\.([^.]+)\.([^.]+)\.([^.]+)", RegexOptions.Compiled);
        public RecipeChanger(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public void Initialize(RecipeData recipeTemplate)
        {
            ActualRecipe = recipeTemplate;
        }
        public async Task WriteToPlc(RecipeData recipe)
        {
            List<DataNode> nodesToWrite = new();
            foreach (var module in recipe.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params)
                    {
                        string path = GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        nodesToWrite.Add(new DataNode() { NodeId = path, Value = parameter.Value });
                    }
                }
            }
            await plcDataAccess.WriteDataNodes(nodesToWrite);
            ActualRecipe = recipe;
        }
        public async Task<RecipeData> ReadFromPlc()
        {
            if (ActualRecipe is null) throw new InvalidOperationException("Call Initialize with reference Recipe before reading from PLC");
            RecipeData recipe = DeepCopier.Copy(ActualRecipe);
            foreach (var module in recipe.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params)
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
        public async Task<RecipeData> Subscribe()
        {
            if (ActualRecipe is null) throw new InvalidOperationException("Call Initialize with reference Recipe before reading from PLC");
            List<string> paths = new();
            RecipeData recipe = DeepCopier.Copy(ActualRecipe);
            foreach (var module in recipe.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params)
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
            var module = ActualRecipe.Modules.Where(x => x.Name == match.Groups[2].Value).FirstOrDefault();
            var station=module.Stations.Where(x => x.Name == match.Groups[3].Value).FirstOrDefault();
            var parameter = station.Params.Where(x => x.Name == match.Groups[4].Value).FirstOrDefault();
            if(parameter.Value != subject.Value) 
            {
                parameter.Value = subject.Value;
                ActualRecipeChanged?.Invoke(this, ActualRecipe);
            }    
        }
    }
}
