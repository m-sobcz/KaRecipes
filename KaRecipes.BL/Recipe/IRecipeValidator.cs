using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public interface IRecipeValidator
    {
        event EventHandler<string> UnknownParameterFound;
        event EventHandler<string> UnsetParameterFound;

        RecipeData Validate(RawRecipe sourceRecipe, Dictionary<string, Type> recipeNodes);
    }
}