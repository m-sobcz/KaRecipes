using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public interface IRecipeValidator
    {
        event EventHandler<string> RemovedUnknownParameter;

        Task<RecipeData> Validate(RawRecipe sourceRecipe);
    }
}