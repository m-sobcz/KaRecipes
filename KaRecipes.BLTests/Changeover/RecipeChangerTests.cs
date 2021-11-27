using Xunit;
using KaRecipes.BL.Changeover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
using Moq;
using KellermanSoftware.CompareNetObjects;
using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Data;
using KaRecipes.BL.Recipe;

namespace KaRecipes.BL.Changeover.Tests
{
    public class RecipeChangerTests
    {
        [Fact()]
        public void Update_CorrectData_UpdatesActualRecipeAndTriggersEvent() 
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            RecipeData recipe = GetSampleRecipe();
            RecipeData eventRecipe = new();
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            recipeChanger.ActualRecipeChanged += (sender, recipe) => eventRecipe = recipe;
            //Act
            
            PlcDataReceivedEventArgs plcDataReceivedEventArgs = new() { Name = "KaRecipes.M01.DB_00_Parameters.single11",Value=123};
            recipeChanger.Initialize(GetSampleRecipe());
            recipeChanger.Update(plcDataReceivedEventArgs);
            //Assert
            Assert.Equal(123, recipeChanger.ActualRecipe.Modules
                .Where(x => x.Name == "M01").FirstOrDefault().Stations
                .Where(x => x.Name == "DB_00_Parameters").FirstOrDefault().Params
                .Where(x => x.Name == "single11").FirstOrDefault().Value);
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(recipeChanger.ActualRecipe, eventRecipe);
            Assert.True(comparison.AreEqual);
        }

        private void RecipeChanger_ActualRecipeChanged(object sender, RecipeData e)
        {
            throw new NotImplementedException();
        }

        [Fact()]
        public void WriteToPlc_AllNodesCorrect_ExecutesWithoutFails()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            List<DataNode> executedNodes = new();
            mockPlcDataAccess.Setup(x => x.WriteDataNodes(It.IsAny<List<DataNode>>())).Callback<List<DataNode>>(x=>executedNodes.AddRange(x)).ReturnsAsync(true);
            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");

            List<DataNode> expectedNodes = new();

            expectedNodes.Add(new DataNode() { NodeId= "KaRecipes.M01.DB_00_Parameters.single11",Value="11" });
            expectedNodes.Add(new DataNode() { NodeId = "KaRecipes.M01.DB_00_Parameters.single12", Value = "12" });
            expectedNodes.Add(new DataNode() { NodeId = "KaRecipes.M02.DB_00_Parameters.single21", Value = "21" });
            expectedNodes.Add(new DataNode() { NodeId = "KaRecipes.M02.DB_00_Parameters.single22", Value = "22" });

            RecipeData recipe = GetSampleRecipe();

            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);

            recipeChanger.WriteToPlc(recipe).Wait();
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedNodes, executedNodes);
            Assert.True(comparison.AreEqual);
        }
        [Fact()]
        public void WriteToPlc_IncorrectNodes_RaiseEvents()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.WriteDataNodes(It.IsAny<List<DataNode>>())).ReturnsAsync(false);

            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");

            RecipeData recipe = GetSampleRecipe();

            HashSet<string> expectedNodes = new();
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single11");
            expectedNodes.Add("KaRecipes.M01.DB_00_Parameters.single12");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single21");
            expectedNodes.Add("KaRecipes.M02.DB_00_Parameters.single22");

            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            HashSet<string> actualFailedNodes = new();
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
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>())).ReturnsAsync(new SingleParamData() { Name = "test", Value = "1" });
            List<StationData> stations1 = new();
            SingleParamData single11 = new() { Name = "single11", Value = "1" };
            SingleParamData single12 = new() { Name = "single12", Value = "1" };
            List<SingleParamData> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "DB_00_Parameters", Params = singles1 });

            List<StationData> stations2 = new();
            SingleParamData single21 = new() { Name = "single21", Value = "1" };
            SingleParamData single22 = new() { Name = "single22", Value = "1" };
            List<SingleParamData> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "DB_00_Parameters", Params = singles2 });

            RecipeData expectedRecipe = new();
            expectedRecipe.Modules = new();
            expectedRecipe.Modules.Add(new ModuleData() { Name = "M01", Stations = stations1 });
            expectedRecipe.Modules.Add(new ModuleData() { Name = "M02", Stations = stations2 });
            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object);
            recipeChanger.Initialize(GetSampleRecipe());
            RecipeData actualRecipe = recipeChanger.ReadFromPlc().Result;
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedRecipe, actualRecipe);
            Assert.True(comparison.AreEqual);
        }
        RecipeData GetSampleRecipe() 
        {
            List<StationData> stations1 = new();
            SingleParamData single11 = new() { Name = "single11", Value = "11" };
            SingleParamData single12 = new() { Name = "single12", Value = "12" };
            List<SingleParamData> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "DB_00_Parameters", Params = singles1 });

            List<StationData> stations2 = new();
            SingleParamData single21 = new() { Name = "single21", Value = "21" };
            SingleParamData single22 = new() { Name = "single22", Value = "22" };
            List<SingleParamData> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "DB_00_Parameters", Params = singles2 });

            RecipeData recipe = new();
            recipe.Modules = new();
            recipe.Modules.Add(new ModuleData() { Name = "M01", Stations = stations1 });
            recipe.Modules.Add(new ModuleData() { Name = "M02", Stations = stations2 });
            return recipe;
        }
    }
}