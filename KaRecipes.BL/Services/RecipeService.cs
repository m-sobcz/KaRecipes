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

        private Recipe _actualRecipe;

        public Recipe ActualRecipe => recipeChanger.ActualRecipe;

        public RecipeService(IRecipeChanger recipeChanger, IRecipeValidator recipeValidator,
            IRawRecipeSerializer recipeSerialzier, IDbDataAccess<Recipe> dbDataAccess)
        {
            this.recipeChanger = recipeChanger;
            this.recipeValidator = recipeValidator;
            this.recipeSerializer = recipeSerialzier;
            this.dbDataAccess = dbDataAccess;
            recipeChanger.ActualRecipeChanged += RecipeChanger_ActualRecipeChanged;
        }

        private void RecipeChanger_ActualRecipeChanged(object sender, Recipe e)
        {
            throw new NotImplementedException();
            //Task.Run(()=>dbDataAccess.Write(recipeChanger.ActualRecipe));
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
            await dbDataAccess.Write(recipeChanger.ActualRecipe);
        } 
        string Serialize()
        {
            string serializedRecipe=recipeSerializer.Serialize(ActualRecipe);
            return serializedRecipe;
        }
    }
}
