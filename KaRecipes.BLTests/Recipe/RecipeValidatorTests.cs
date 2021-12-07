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
            RawRecipe inputRecipe = GetSampleRecipe();
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
            RawRecipe inputRecipe = GetSampleRecipe();
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
            RawRecipe inputRecipe = GetSampleRecipe();
            inputRecipe.ParameterModules.First().Stations.First().Params.RemoveAt(0);
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

        RawRecipe GetSampleRecipe()
        {

            List<StationData> stations1 = new();
            SingleParamData single11 = new() { Name = "single11", Value = "11" };
            SingleParamData single12 = new() { Name = "single12", Value = "12" };
            List<SingleParamData> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "M01_DB_00_Parameters", Params = singles1 });

            List<StationData> stations2 = new();
            SingleParamData single21 = new() { Name = "single21", Value = "21" };
            SingleParamData single22 = new() { Name = "single22", Value = "22" };
            List<SingleParamData> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "M02_DB_00_Parameters", Params = singles2 });

            RawRecipe recipe = new();
            recipe.ParameterModules = new();
            recipe.ParameterModules.Add(new ModuleData() { Name = "M01", Stations = stations1 });
            recipe.ParameterModules.Add(new ModuleData() { Name = "M02", Stations = stations2 });
            return recipe;
        }
    }
}