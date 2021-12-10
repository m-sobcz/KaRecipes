
using KaRecipes.BL.Data.RecipeAggregate;

namespace KaRecipes.BL.Recipe
{
    public interface IRawRecipeSerializer
    {
        RawRecipeData Deserialize(string text);
        void FillRecipeWithHeaderInfo(RawRecipeData recipe, string headerInfo);
        string Serialize(RecipeData recipe);
    }
}