using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using System;
using System.Threading.Tasks;

namespace KaRecipes.BL.Changeover
{
    public interface IRecipeChanger : IObserver
    {
        event EventHandler<string> WriteToNodeFailed;
        event EventHandler<PlcDataReceivedEventArgs> ActualRecipeChanged;
        public Recipe ActualRecipe { get; set; }
        void Initialize(Recipe recipeTemplate);
        Task<Recipe> ReadFromPlc();
        Task WriteToPlc(Recipe recipe);
    }
}