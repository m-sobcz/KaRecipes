using KaRecipes.BL.RecipeAggregate;

namespace KaRecipes.BL.Serialize
{
    public interface IRecipeSerializer
    {
        Recipe Deserialize(string text);
        void FillRecipeWithHeaderInfo(Recipe recipe, string headerInfo);
        string Serialize(Recipe recipe);
    }
}