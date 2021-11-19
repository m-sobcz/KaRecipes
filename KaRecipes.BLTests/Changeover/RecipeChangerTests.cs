using Xunit;
using KaRecipes.BL.Changeover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KaRecipes.BL.RecipeAggregate;
using KellermanSoftware.CompareNetObjects;

namespace KaRecipes.BL.Changeover.Tests
{
    public class RecipeChangerTests
    {
        [Fact()]
        public void Update_CorrectData_UpdatesActualRecipeAndTriggersEvent() 
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            Recipe recipe = GetSampleRecipe();
            Recipe eventRecipe = new();
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            recipeChanger.ActualRecipeChanged += (sender, recipe) => eventRecipe = recipe;
            //Act
            
            PlcDataReceivedEventArgs plcDataReceivedEventArgs = new() { Name = "KaRecipes.M01.DB_00_Parameters.single11",Value=123};
            recipeChanger.Initialize(GetSampleRecipe());
            recipeChanger.Update(plcDataReceivedEventArgs);
            //Assert
            Assert.Equal(123, recipeChanger.ActualRecipe.ParameterModules
                .Where(x => x.Name == "M01").FirstOrDefault().ParameterStations
                .Where(x => x.Name == "DB_00_Parameters").FirstOrDefault().ParameterSingles
                .Where(x => x.Name == "single11").FirstOrDefault().Value);
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(recipeChanger.ActualRecipe, eventRecipe);
            Assert.True(comparison.AreEqual);
        }

        private void RecipeChanger_ActualRecipeChanged(object sender, Recipe e)
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void WriteToPlc_AllNodesCorrect_ExecutesWithoutFails()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            Dictionary<string, string> executedNodes = new();
            mockPlcDataAccess.Setup(x => x.WriteParameter(It.IsAny<string>(), It.IsAny<object>())).Callback<string, object>((x, y) => executedNodes.Add(x,y.ToString())).ReturnsAsync(true);
            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");

            Dictionary<string, string> expectedNodes = new();
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single11", "11");
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single12", "12");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single21", "21");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single22", "22");

            Recipe recipe = GetSampleRecipe();

            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            HashSet<string> actualFailedNodes = new();
            recipeChanger.WriteToNodeFailed += (sender, nodeId) => actualFailedNodes.Add(nodeId);
            recipeChanger.WriteToPlc(recipe).Wait();
            //Assert
            Assert.Empty(actualFailedNodes);
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedNodes, executedNodes);
            Assert.True(comparison.AreEqual);
        }
        [Fact()]
        public void WriteToPlc_IncorrectNodes_RaiseEvents()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.WriteParameter(It.IsAny<string>(), It.IsAny<object>())).ReturnsAsync(false);

            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");

            Recipe recipe = GetSampleRecipe();

            HashSet<string> expectedNodes = new();
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single11");
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single12");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single21");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single22");

            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            HashSet<string> actualFailedNodes = new();
            recipeChanger.WriteToNodeFailed += (sender, nodeId) => actualFailedNodes.Add(nodeId);
            recipeChanger.WriteToPlc(recipe).Wait();
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedNodes, actualFailedNodes);
            Assert.True(comparison.AreEqual);
        }
        [Fact()]
        public void ReadFromPlc_WithoutInitialization_ThrowsInvalidOpException() 
        {
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            Assert.ThrowsAsync<InvalidOperationException>(()=>recipeChanger.ReadFromPlc());
        }
        [Fact()]
        public void ReadFromPlc_ProperExecution()
        {
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>())).ReturnsAsync(new ParameterSingle() { Name = "test", Value = "1" });
            List<ParameterStation> stations1 = new();
            ParameterSingle single11 = new() { Name = "single11", Value = "1" };
            ParameterSingle single12 = new() { Name = "single12", Value = "1" };
            List<ParameterSingle> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "DB_00_Parameters", ParameterSingles = singles1 });

            List<ParameterStation> stations2 = new();
            ParameterSingle single21 = new() { Name = "single21", Value = "1" };
            ParameterSingle single22 = new() { Name = "single22", Value = "1" };
            List<ParameterSingle> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "DB_00_Parameters", ParameterSingles = singles2 });

            Recipe expectedRecipe = new();
            expectedRecipe.ParameterModules = new();
            expectedRecipe.ParameterModules.Add(new ParameterModule() { Name = "M01", ParameterStations = stations1 });
            expectedRecipe.ParameterModules.Add(new ParameterModule() { Name = "M02", ParameterStations = stations2 });
            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            recipeChanger.Initialize(GetSampleRecipe());
            Recipe actualRecipe = recipeChanger.ReadFromPlc().Result;
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedRecipe, actualRecipe);
            Assert.True(comparison.AreEqual);
        }
        Recipe GetSampleRecipe() 
        {
            List<ParameterStation> stations1 = new();
            ParameterSingle single11 = new() { Name = "single11", Value = "11" };
            ParameterSingle single12 = new() { Name = "single12", Value = "12" };
            List<ParameterSingle> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "DB_00_Parameters", ParameterSingles = singles1 });

            List<ParameterStation> stations2 = new();
            ParameterSingle single21 = new() { Name = "single21", Value = "21" };
            ParameterSingle single22 = new() { Name = "single22", Value = "22" };
            List<ParameterSingle> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "DB_00_Parameters", ParameterSingles = singles2 });

            Recipe recipe = new();
            recipe.ParameterModules = new();
            recipe.ParameterModules.Add(new ParameterModule() { Name = "M01", ParameterStations = stations1 });
            recipe.ParameterModules.Add(new ParameterModule() { Name = "M02", ParameterStations = stations2 });
            return recipe;
        }
    }
}