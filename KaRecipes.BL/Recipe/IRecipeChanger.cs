using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public interface IRecipeChanger : IObserver
    {
        event EventHandler<RecipeData> ActualRecipeChanged;
        public RecipeData ActualRecipe { get; set; }
        void Initialize(RecipeData recipeTemplate);
        Task<RecipeData> ReadFromPlc();
        Task WriteToPlc(RecipeData recipe);
    }
}