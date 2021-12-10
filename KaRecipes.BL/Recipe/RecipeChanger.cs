using DeepCopy;
using KaRecipes.BL.Data;
using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.Utils;
using KellermanSoftware.CompareNetObjects;
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
        Dictionary<string, SingleParamData> SingleParameters { get; set; }
        CompareLogic compare = new();

        public int PublishingInterval => 1000;

        readonly IPlcDataAccess plcDataAccess;
        public event EventHandler<RecipeData> ActualRecipeChanged;
        Regex regex = new(@"([^.]+)\.([^.]+)\.([^.]+)\.([^.]+)", RegexOptions.Compiled);
        public RecipeChanger(IPlcDataAccess plcDataAccess, RecipeData recipe)
        {
            SingleParameters = new();
            foreach (var module in recipe.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params)
                    {
                        SingleParameters.Add(parameter.NodeId, parameter);
                    }
                }
            }
            ActualRecipe = recipe;
            this.plcDataAccess = plcDataAccess;
        }
        public async Task WriteToPlc(RecipeData recipe)
        {
            var nodesToWrite = recipe.Modules.SelectMany(x => x.Stations).SelectMany(x => x.Params).Select(x => x as DataNode).ToList();
            await plcDataAccess.WriteDataNodes(nodesToWrite);
            ActualRecipe = recipe;
        }
        public async Task<RecipeData> ReadFromPlc()
        {
            RecipeData recipe = DeepCopier.Copy(ActualRecipe);
            foreach (var module in recipe.Modules)
            {
                foreach (var station in module.Stations)
                {
                    foreach (var parameter in station.Params)
                    { 
                        var node = await plcDataAccess.ReadDataNode(parameter.NodeId);
                        parameter.Value = node.Value;
                    }
                }
            }
            ActualRecipe = recipe;
            return recipe;
        }
        public async Task<RecipeData> Subscribe(RecipeData recipe)
        {         
            await plcDataAccess.CreateSubscriptionsWithInterval(SingleParameters.Keys.ToList(), PublishingInterval, this);
            return recipe;
        }

        string GetNodeIdentifier(string module, string station, string parameter)
        {
            string path = $"{plcDataAccess.PlcAccessPrefix}.{module}.{station}.{parameter}";
            return path;
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            if (SingleParameters.TryGetValue(subject.Name, out var singleParam))
            {
                bool valueChanged = compare.Compare(singleParam.Value, subject.Value).AreEqual == false;
                singleParam.Value = subject.Value;
                if (valueChanged)
                {
                    singleParam.Value = subject.Value;
                    ActualRecipeChanged?.Invoke(this, ActualRecipe);
                }
            }
            else
            {
                Traceability.Notify(subject);
            }
        }

    }
}
