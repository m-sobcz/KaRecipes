using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using KellermanSoftware.CompareNetObjects;

using KaRecipes.BL.Recipe;
using KaRecipes.BL.Data.RecipeAggregate;

namespace KaRecipes.BL.Recipes.Tests
{
    public class RecipeSerializerTests
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
      <ParameterGroup name=""M11_DB_01_Param"">
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
            var expected = new RawRecipe() {
                ParameterModules = new List<ModuleData>
            {
                new ModuleData()
                {
                   Name="M11",
                   Stations=new List<StationData>()
                   {
                       new StationData()
                       {
                           Name="M11_DB_00_Parameters",
                           Params=new List<SingleParamData>()
                           {
                               new SingleParamData()
                               {
                                   Name="ActualType",
                                   Value="0"
                               },
                               new SingleParamData()
                               {
                                   Name="MachineCycleTime",
                                   Value="4500"
                               }
                           }
                       },
                       new StationData()
                       {
                           Name="M11_DB_01_Param",
                           Params=new List<SingleParamData>()
                           {
                               new SingleParamData()
                               {
                                   Name="LR_LR_aktiv",
                                   Value="False"
                               }
                           }
                       }
                   }
                },
                new ModuleData()
                {
                   Name="M15",
                   Stations=new List<StationData>()
                }
            }
            };
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
            var result=recipeSerializer.Deserialize(xml);
            Assert.Empty(result.ParameterModules);
        }
        [Fact()]
        public void Serialize_SampleData()
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
            RawRecipe recipe = new();
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"C:\Users\MISO\Desktop\2up_12345.xp0");
            Assert.Equal("2up", recipe.Name);
            Assert.Equal(12345, recipe.VersionId);
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"D:\Users\MISO\Desktop\2upz.xp0");
        }

    }
}