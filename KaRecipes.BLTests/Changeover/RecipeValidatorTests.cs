using Xunit;
using KaRecipes.BL.Changeover;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;

namespace KaRecipes.BL.Changeover.Tests
{
    public class RecipeValidatorTests
    {
        
        [Fact()]
        public void Validate_AllFieldsExist_JustCallsReadParam()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>()));
            Dictionary<string, string> nodes = new();
            nodes.Add("KaRecipes.M01.DB_00_Parameters.single11", "single11");
            nodes.Add("KaRecipes.M01.DB_00_Parameters.single12", "single12");
            nodes.Add("KaRecipes.M02.DB_00_Parameters.single21", "single21");
            nodes.Add("KaRecipes.M02.DB_00_Parameters.single22", "single22");
            mockPlcDataAccess.Setup(x => x.GetAvailableNodes()).Returns(nodes);
            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>())).Returns(Task.FromResult(new DataNode() {Value="101"}));
            RecipeValidator recipeVerificator = new(mockPlcDataAccess.Object);
            RawRecipe inputRecipe = GetSampleRecipe();
            //Act
            HashSet<string> reportedNodes = new();
            recipeVerificator.RemovedUnknownParameter += (sender, nodeId) => reportedNodes.Add(nodeId);
            Recipe convertedRecipe=recipeVerificator.Validate(inputRecipe).Result;
            //Assert
            mockPlcDataAccess.Verify(mock => mock.ReadDataNode(It.IsAny<string>()), Times.Exactly(4));
            Assert.Empty(reportedNodes);
        }
        [Fact()]
        public void Validate_OneFieldDoesntExist_CallsReadAndTriggersEvent()
        {
            //Arrange
            Mock<IPlcDataAccess> mockPlcDataAccess = new Mock<IPlcDataAccess>();
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>()));
            Dictionary<string, string> nodes = new();
            nodes.Add("KaRecipes.M01.DB_00_Parameters.single11", "single11");
            nodes.Add("KaRecipes.M01.DB_00_Parameters.single12", "single12");
            nodes.Add("KaRecipes.M02.DB_00_Parameters.single21", "single21");
            mockPlcDataAccess.Setup(x => x.GetAvailableNodes()).Returns(nodes);
            mockPlcDataAccess.SetupGet(x => x.PlcAccessPrefix).Returns("KaRecipes");
            mockPlcDataAccess.Setup(x => x.ReadDataNode(It.IsAny<string>())).Returns(Task.FromResult(new DataNode() { Value = "101" }));
            RecipeValidator recipeVerificator = new(mockPlcDataAccess.Object);
            RawRecipe inputRecipe = GetSampleRecipe();
            //Act
            HashSet<string> removedNodes = new();
            recipeVerificator.RemovedUnknownParameter += (sender, nodeId) => removedNodes.Add(nodeId);
            Recipe convertedRecipe = recipeVerificator.Validate(inputRecipe).Result;
            //Assert
            //mockPlcDataAccess.Verify(mock => mock.ReadDataNode(It.IsAny<string>()), Times.Exactly(3));
            Assert.Contains("KaRecipes.M02.DB_00_Parameters.single22", removedNodes);
        }


        RawRecipe GetSampleRecipe() 
        {

            List<ParameterStation> stations1 = new();
            ParameterSingle single11 = new() { Name = "single11", Value = "11" };
            ParameterSingle single12 = new() { Name = "single12", Value = "12" };
            List<ParameterSingle> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "M01_DB_00_Parameters", ParameterSingles = singles1 });

            List<ParameterStation> stations2 = new();
            ParameterSingle single21 = new() { Name = "single21", Value = "21" };
            ParameterSingle single22 = new() { Name = "single22", Value = "22" };
            List<ParameterSingle> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "M02_DB_00_Parameters", ParameterSingles = singles2 });

            RawRecipe recipe = new();
            recipe.ParameterModules = new();
            recipe.ParameterModules.Add(new ParameterModule() { Name = "M01", ParameterStations = stations1 });
            recipe.ParameterModules.Add(new ParameterModule() { Name = "M02", ParameterStations = stations2 });
            return recipe;
        }
    }
}