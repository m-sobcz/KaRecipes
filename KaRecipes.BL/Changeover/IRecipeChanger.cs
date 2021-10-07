using KaRecipes.BL.RecipeAggregate;
using System;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public interface IRecipeChanger
    {
        event EventHandler<string> WriteToNodeFailed;

        void Initialize(Recipe recipeTemplate);
        Task<Recipe> ReadFromPlc();
        Task WriteToPlc(Recipe recipe);
    }
}