using KaRecipes.BL.RecipeAggregate;
using System;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public interface IRecipeValidator
    {
        event EventHandler<string> RemovedUnknownParameter;

        Task<Recipe> Validate(RawRecipe sourceRecipe);
    }
}