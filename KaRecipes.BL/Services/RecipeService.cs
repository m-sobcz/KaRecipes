using KaRecipes.BL.Changeover;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Serialize;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class RecipeService
    {
        IRecipeChanger recipeChanger;
        IRecipeValidator recipeValidator;
        IDbDataAccess<Recipe> dbDataAccess;
        IRawRecipeSerializer recipeSerializer;
        Recipe fileRecipe;
        Recipe plcRecipe;

        private Recipe _actualRecipe;

        public Recipe ActualRecipe
        {
            get { return _actualRecipe; }
            set { _actualRecipe = value; }
        }

        public RecipeService(IRecipeChanger recipeChanger, IRecipeValidator recipeValidator,
            IRawRecipeSerializer recipeSerialzier, IDbDataAccess<Recipe> dbDataAccess)
        {
            this.recipeChanger = recipeChanger;
            this.recipeValidator = recipeValidator;
            this.recipeSerializer = recipeSerialzier;
            this.dbDataAccess = dbDataAccess;
        }
        async Task<Recipe> Deserialize(string serialized) 
        {
            RawRecipe rawRecipe =recipeSerializer.Deserialize(serialized);
            fileRecipe = await recipeValidator.Validate(rawRecipe);
            ActualRecipe = fileRecipe;
            recipeChanger.Initialize(fileRecipe);
            return fileRecipe;
        }
        string Serialize()
        {
            string serializedRecipe=recipeSerializer.Serialize(ActualRecipe);
            return serializedRecipe;
        }
        async Task<Recipe> ReadFromPlc()
        {
            plcRecipe=await recipeChanger.ReadFromPlc();
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(ActualRecipe, plcRecipe);
            if (comparison.AreEqual == false)
            {
                ActualRecipe = plcRecipe;
            }
            return plcRecipe;
        }
    }
}
