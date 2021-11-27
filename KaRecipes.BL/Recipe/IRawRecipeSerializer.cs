
using KaRecipes.BL.Data.RecipeAggregate;

namespace KaRecipes.BL.Recipe
{
    public interface IRawRecipeSerializer
    {
        RawRecipe Deserialize(string text);
        void FillRecipeWithHeaderInfo(RawRecipe recipe, string headerInfo);
        string Serialize(RecipeData recipe);
    }
}