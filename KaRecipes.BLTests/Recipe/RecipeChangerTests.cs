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
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object, recipe);
            recipeChanger.ActualRecipeChanged += (sender, recipe) => eventRecipe = recipe;
            //Act            
            PlcDataReceivedEventArgs plcDataReceivedEventArgs = new() { Name = "KaRecipes.M01.DB_00_Parameters.single11",Value="123"};
            recipeChanger.Subscribe(GetSampleRecipe()).Wait();
            recipeChanger.Update(plcDataReceivedEventArgs);
            //Assert
            var param = recipeChanger.ActualRecipe.Modules[0].Stations[0].Params[0];
            Assert.Equal("123", recipeChanger.ActualRecipe.Modules
                .Where(x => x.Name == "M01").FirstOrDefault().Stations
                .Where(x => x.Name == "DB_00_Parameters").FirstOrDefault().Params
                .Where(x => x.Name == "single11").FirstOrDefault().Value);
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(recipeChanger.ActualRecipe, eventRecipe);
            Assert.True(comparison.AreEqual);
        }


        [Fact()]
        public void WriteToPlc_AllNodesCorrect_ExecutesWithoutFails()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            List<DataNode> executedNodes = new();
            mockPlcDataAccess.Setup(x => x.WriteDataNodes(It.IsAny<List<DataNode>>())).Callback<List<DataNode>>(x=>executedNodes.AddRange(x)).ReturnsAsync(true);
            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");

            RecipeData recipe = GetSampleRecipe();

            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object,recipe);
            List<DataNode> recipeNodes = recipe.Modules.
                SelectMany(x => x.Stations).
                SelectMany(x => x.Params).
                Select(x=> x as DataNode).
                ToList();
            //List<DataNode> recipeNodes = x.Select(x => x as DataNode).ToList();
            recipeChanger.WriteToPlc(recipe).Wait();
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(recipeNodes, executedNodes);
            Assert.True(comparison.AreEqual);
        }

        [Fact()]
        public void ReadFromPlc_ProperExecution()
        {
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.ReadDataNode("KaRecipes.M01.DB_00_Parameters.single11")).ReturnsAsync(new DataNode() { Value = "111" });
            mockPlcDataAccess.Setup(x => x.ReadDataNode("KaRecipes.M01.DB_00_Parameters.single12")).ReturnsAsync(new DataNode() { Value = "112" });
            mockPlcDataAccess.Setup(x => x.ReadDataNode("KaRecipes.M02.DB_00_Parameters.single21")).ReturnsAsync(new DataNode() { Value = "121" });
            mockPlcDataAccess.Setup(x => x.ReadDataNode("KaRecipes.M02.DB_00_Parameters.single22")).ReturnsAsync(new DataNode() { Value = "122" });

            RecipeData expectedRecipe = new();
            ModuleData module1 = expectedRecipe.AddModule("M01");
            var station1 = module1.AddStation("DB_00_Parameters");
            station1.AddParam("single11", "KaRecipes.M01.DB_00_Parameters.single11", "111");
            station1.AddParam("single12", "KaRecipes.M01.DB_00_Parameters.single12", "112");
            ModuleData module2 = expectedRecipe.AddModule("M02");
            var station2 = module2.AddStation("DB_00_Parameters");
            station2.AddParam("single21", "KaRecipes.M02.DB_00_Parameters.single21", "121");
            station2.AddParam("single22", "KaRecipes.M02.DB_00_Parameters.single22", "122");
            //Act
            RecipeChanger recipeChanger = new(mockPlcDataAccess.Object, GetSampleRecipe());
            RecipeData actualRecipe = recipeChanger.ReadFromPlc().Result;
            //Assert
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expectedRecipe, actualRecipe);
            Assert.True(comparison.AreEqual);
        }
        RecipeData GetSampleRecipe() 
        {
            RecipeData recipe = new();
            ModuleData module1 = recipe.AddModule("M01");
            var station1 = module1.AddStation("DB_00_Parameters");
            station1.AddParam("single11", "KaRecipes.M01.DB_00_Parameters.single11", "11");
            station1.AddParam("single12", "KaRecipes.M01.DB_00_Parameters.single12", "12");
            ModuleData module2 = recipe.AddModule("M02");
            var station2 = module2.AddStation("DB_00_Parameters");
            station2.AddParam("single21", "KaRecipes.M02.DB_00_Parameters.single21", "21");
            station2.AddParam("single22", "KaRecipes.M02.DB_00_Parameters.single22", "22");
            return recipe;
        }
    }
}