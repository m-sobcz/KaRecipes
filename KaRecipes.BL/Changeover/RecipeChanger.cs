using DeepCopy;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public class RecipeChanger
    {
        Recipe ActualRecipe { get; set; }
        readonly IPlcDataAccess plcDataAccess;  
        public event EventHandler<string> WriteToNodeFailed;
        public RecipeChanger(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public async Task Initialize(Recipe recipeTemplate) 
        {
            ActualRecipe = recipeTemplate;
            ActualRecipe = await ReadFromPlc();
        }
        public async Task WriteToPlc(Recipe recipe) 
        {
             foreach (var module in recipe.ParameterModules)
            {
                foreach (var station in module.ParameterStations)
                {
                    foreach (var parameter in station.ParameterSingles)
                    {
                        string path=PlcNode.GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        bool writingOk=await plcDataAccess.WriteParameter(path, parameter.Value);
                        if (writingOk == false) OnWriteToNodeFailed(path);
                    }
                }
            }
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
                        string path = PlcNode.GetNodeIdentifier(module.Name, station.Name, parameter.Name);
                        var node=await plcDataAccess.ReadParameter(path);
                        parameter.Value = node.ToString();
                    }
                }
            }
            return recipe;
        }
        
        void OnWriteToNodeFailed(string path)
        {
            WriteToNodeFailed?.Invoke(this, path);
        }
    }
}
