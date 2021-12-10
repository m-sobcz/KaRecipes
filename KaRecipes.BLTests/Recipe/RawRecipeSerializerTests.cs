using Xunit;
using KaRecipes.BL.Recipe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaRecipes.BL.Data.RecipeAggregate;
using KellermanSoftware.CompareNetObjects;

namespace KaRecipes.BL.Recipe.Tests
{
    public class RawRecipeSerializerTests
    {
        [Fact()]
        public void Deserialize_SampleData_AreEqual()
        {
            string xml = @"<Parameters>
  <ParameterGroups>
    <ParameterGroup name=""M11"">
      <ParameterGroup name=""M11_DB_00_Parameters"">
        <Parameter name=""ActualType"" value=""0"" />
        <Parameter name=""MachineCycleTime"" value=""4500"" />
      </ParameterGroup>
      <ParameterGroup name=""M11_DB_01_Parameters"">
        <Parameter name=""LR_LR_aktiv"" value=""False"" />
      </ParameterGroup>
    </ParameterGroup>
    <ParameterGroup name=""M15"">
    </ParameterGroup>
  </ParameterGroups>
</Parameters>
";
            RawRecipeSerializer recipeSerializer = new();
            var result = recipeSerializer.Deserialize(xml);
            var expected = new RawRecipeData();
            ModuleData M11 = new("M11");
            var M11_DB_00_Parameters=M11.AddStation("M11_DB_00_Parameters");
            M11_DB_00_Parameters.AddParam("ActualType", null, "0");
            M11_DB_00_Parameters.AddParam("MachineCycleTime", null, "4500");
            var M11_DB_01_Parameters = M11.AddStation("M11_DB_01_Parameters");
            M11_DB_01_Parameters.AddParam("LR_LR_aktiv", null, "False");
            ModuleData M15 = new("M15");
            expected.Modules.Add(M11);
            expected.Modules.Add(M15);
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expected, result);
            Assert.True(comparison.AreEqual);
        }
        [Fact()]
        public void Deserialize_Empty_IsEmpty()
        {
            string xml =
@"<Parameters>
  <ParameterGroups>
  </ParameterGroups>
</Parameters>";
            RawRecipeSerializer recipeSerializer = new();
            var result = recipeSerializer.Deserialize(xml);
            Assert.Empty(result.Modules);
        }
        [Fact()]
        public void Serialize_SampleData()
        {
            RecipeData recipe = new();
            ModuleData module1 = new ModuleData("M01");
            var station1 = module1.AddStation("DB_00_Parameters");
            station1.AddParam("single11", "KaRecipes.M01.single11", "11");
            station1.AddParam("single12", "KaRecipes.M01.single12", "12");
            ModuleData module2 = new ModuleData("M02");
            var station2 = module2.AddStation("DB_00_Parameters");
            station2.AddParam("single21", "KaRecipes.M02.single21", "21");
            station2.AddParam("single22", "KaRecipes.M02.single22", "22");
            recipe.Modules.Add(module1);
            recipe.Modules.Add(module2);

            RawRecipeSerializer recipeSerializer = new();
            string actual = recipeSerializer.Serialize(recipe);
            string expected =
@"<Parameters>
  <ParameterGroups>
    <ParameterGroup name=""M01"">
      <ParameterGroup name=""M01_DB_00_Parameters"">
        <Parameter name=""single11"" value=""11"" />
        <Parameter name=""single12"" value=""12"" />
      </ParameterGroup>
    </ParameterGroup>
    <ParameterGroup name=""M02"">
      <ParameterGroup name=""M02_DB_00_Parameters"">
        <Parameter name=""single21"" value=""21"" />
        <Parameter name=""single22"" value=""22"" />
      </ParameterGroup>
    </ParameterGroup>
  </ParameterGroups>
</Parameters>";
            Assert.Equal(expected, actual);
        }
        [Fact()]
        public void FillRecipeWithHeaderInfo_DataCorrect_SetsNameAndVersionId()
        {
            RawRecipeSerializer recipeSerializer = new();
            RawRecipeData recipe = new();
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"C:\Users\MISO\Desktop\2up_12345.xp0");
            Assert.Equal("2up", recipe.Name);
            Assert.Equal(12345, recipe.VersionId);
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"D:\Users\MISO\Desktop\2upz.xp0");
        }
    }
}