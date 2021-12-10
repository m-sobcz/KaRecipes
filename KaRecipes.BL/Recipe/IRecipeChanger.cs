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
        Task<RecipeData> ReadFromPlc();
        Task WriteToPlc(RecipeData recipe);
        Task<RecipeData> Subscribe(RecipeData recipe);
    }
}