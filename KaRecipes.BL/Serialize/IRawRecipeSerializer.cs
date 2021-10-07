using KaRecipes.BL.RecipeAggregate;

namespace KaRecipes.BL.Serialize
{
    public interface IRawRecipeSerializer
    {
        RawRecipe Deserialize(string text);
        void FillRecipeWithHeaderInfo(RawRecipe recipe, string headerInfo);
        string Serialize(Recipe recipe);
    }
}