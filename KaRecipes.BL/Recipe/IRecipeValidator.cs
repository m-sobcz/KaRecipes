﻿using KaRecipes.BL.Data.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Recipe
{
    public interface IRecipeValidator
    {
        RecipeData Validate(RawRecipe sourceRecipe, Dictionary<string, Type> recipeNodes);
    }
}