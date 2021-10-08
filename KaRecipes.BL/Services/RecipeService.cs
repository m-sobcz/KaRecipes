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
        public Recipe PlcRecipe { get; set; }

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
            recipeChanger.ActualRecipeChanged += RecipeChanger_ActualRecipeChanged;
        }

        private void RecipeChanger_ActualRecipeChanged(object sender, Recipe recipe)
        {
            PlcRecipe = recipe;
            Task.Run(()=>dbDataAccess.Write(PlcRecipe));
        }

        async Task<Recipe> Deserialize(string serialized) 
        {
            RawRecipe rawRecipe =recipeSerializer.Deserialize(serialized);
            fileRecipe = await recipeValidator.Validate(rawRecipe);
            return fileRecipe;
        }
        async Task Load (Recipe recipe) 
        {
            recipeChanger.Initialize(recipe);
            await recipeChanger.WriteToPlc(recipe);
            PlcRecipe = recipe;
            await dbDataAccess.Write(PlcRecipe);
        } 
        string Serialize()
        {
            string serializedRecipe=recipeSerializer.Serialize(ActualRecipe);
            return serializedRecipe;
        }
    }
}
