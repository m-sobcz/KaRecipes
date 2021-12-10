using Xunit;
using KaRecipes.BL.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Data;
using KaRecipes.BL.Interfaces;
using Moq;

namespace KaRecipes.BL.Recipe.Tests
{
    public class RecipeValidatorTests
    {
        [Fact()]
        public void Validate_CorrectRecipe_NoParametersNotFound()
        {
            //Arrange
            RecipeValidator recipeVerificator = new();
            RawRecipeData inputRecipe = GetSampleRecipe();
            Dictionary<string, Type> recipeNodes = new();
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single11", typeof(string));
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single12", typeof(string));
            recipeNodes.Add("KaRecipes.M02.DB_00_Parameters.single21", typeof(string));
            recipeNodes.Add("KaRecipes.M02.DB_00_Parameters.single22", typeof(string));
            //Act
            RecipeData convertedRecipe = recipeVerificator.Validate(inputRecipe, recipeNodes);
            //Assert
            Assert.Empty(convertedRecipe.UnknownParametersFound);
            Assert.Empty(convertedRecipe.UnsetParametersFound);
        }
        [Fact()]
        public void Validate_OneParameterUnset_ExistInCollection()
        {
            //Arrange
            RecipeValidator recipeVerificator = new();
            RawRecipeData inputRecipe = GetSampleRecipe();
            Dictionary<string, Type> recipeNodes = new();
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single11", typeof(string));
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single12", typeof(string));
            recipeNodes.Add("KaRecipes.M02.DB_00_Parameters.single21", typeof(string));
            //Act
            RecipeData convertedRecipe = recipeVerificator.Validate(inputRecipe, recipeNodes);
            //Assert
            Assert.Contains("KaRecipes.M02.DB_00_Parameters.single22", convertedRecipe.UnknownParametersFound);
            Assert.Empty(convertedRecipe.UnsetParametersFound);
        }
        [Fact()]
        public void Validate_OneParameterNotFound_ExistInCollection()
        {
            //Arrange
            RecipeValidator recipeVerificator = new();
            RawRecipeData inputRecipe = GetSampleRecipe();
            inputRecipe.Modules.First().Stations.First().Params.RemoveAt(0);
            Dictionary<string, Type> recipeNodes = new();
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single11", typeof(string));
            recipeNodes.Add("KaRecipes.M01.DB_00_Parameters.single12", typeof(string));
            recipeNodes.Add("KaRecipes.M02.DB_00_Parameters.single21", typeof(string));
            recipeNodes.Add("KaRecipes.M02.DB_00_Parameters.single22", typeof(string));
            //Act
            RecipeData convertedRecipe = recipeVerificator.Validate(inputRecipe, recipeNodes);
            //Assert      
            Assert.Empty(convertedRecipe.UnknownParametersFound);
            Assert.Contains("KaRecipes.M01.DB_00_Parameters.single11", convertedRecipe.UnsetParametersFound);
        }

        RawRecipeData GetSampleRecipe()
        {
            RawRecipeData recipe = new();
            ModuleData module1 = new ModuleData("M01");
            var station1 = module1.AddStation("DB_00_Parameters");
            station1.AddParam("single11", "KaRecipes.M01.DB_00_Parameters.single11", "11");
            station1.AddParam("single12", "KaRecipes.M01.DB_00_Parameters.single12", "12");
            ModuleData module2 = new ModuleData("M02");
            var station2 = module2.AddStation("DB_00_Parameters");
            station2.AddParam("single21", "KaRecipes.M02.DB_00_Parameters.single21", "21");
            station2.AddParam("single22", "KaRecipes.M02.DB_00_Parameters.single22", "22");
            recipe.Modules.Add(module1);
            recipe.Modules.Add(module2);
            return recipe;
        }
    }
}