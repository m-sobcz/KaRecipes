using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.Recipe;
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
        IDbWrite<RecipeData> dbDataAccess;
        IRawRecipeSerializer recipeSerializer;
        RecipeData fileRecipe;

        public RecipeData ActualRecipe => recipeChanger.ActualRecipe;

        public RecipeService(IRecipeChanger recipeChanger, IRecipeValidator recipeValidator,
            IRawRecipeSerializer recipeSerializer, IDbWrite<RecipeData> dbDataAccess)
        {
            this.recipeChanger = recipeChanger;
            this.recipeValidator = recipeValidator;
            this.recipeSerializer = recipeSerializer;
            this.dbDataAccess = dbDataAccess;
            recipeChanger.ActualRecipeChanged += RecipeChanger_ActualRecipeChanged;
        }

        private void RecipeChanger_ActualRecipeChanged(object sender, RecipeData e)
        {
            throw new NotImplementedException();
            //Task.Run(()=>dbDataAccess.Write(recipeChanger.ActualRecipe));
        }

        async Task<RecipeData> Deserialize(string serialized) 
        {
            RawRecipe rawRecipe =recipeSerializer.Deserialize(serialized);
            fileRecipe = await recipeValidator.Validate(rawRecipe);
            return fileRecipe;
        }
        async Task Load (RecipeData recipe) 
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
