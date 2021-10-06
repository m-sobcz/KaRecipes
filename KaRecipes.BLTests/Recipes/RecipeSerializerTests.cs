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
using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Serialize;

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
            RecipeSerializer recipeSerializer = new();
            var result = recipeSerializer.Deserialize(xml);
            var expected = new RawRecipe() {
                ParameterModules = new List<ParameterModule>
            {
                new ParameterModule()
                {
                   Name="M11",
                   ParameterStations=new List<ParameterStation>()
                   {
                       new ParameterStation()
                       {
                           Name="M11_DB_00_Parameters",
                           ParameterSingles=new List<ParameterSingle>()
                           {
                               new ParameterSingle()
                               {
                                   Name="ActualType",
                                   Value="0"
                               },
                               new ParameterSingle()
                               {
                                   Name="MachineCycleTime",
                                   Value="4500"
                               }
                           }
                       },
                       new ParameterStation()
                       {
                           Name="M11_DB_01_Param",
                           ParameterSingles=new List<ParameterSingle>()
                           {
                               new ParameterSingle()
                               {
                                   Name="LR_LR_aktiv",
                                   Value="False"
                               }
                           }
                       }
                   }
                },
                new ParameterModule()
                {
                   Name="M15",
                   ParameterStations=new List<ParameterStation>()
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
            RecipeSerializer recipeSerializer = new();
            var result=recipeSerializer.Deserialize(xml);
            Assert.Empty(result.ParameterModules);
        }
        [Fact()]
        public void Serialize_SampleData()
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

            RecipeSerializer recipeSerializer = new();
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
            RecipeSerializer recipeSerializer = new();
            RawRecipe recipe = new();
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"C:\Users\MISO\Desktop\2up_12345.xp0");
            Assert.Equal("2up", recipe.Name);
            Assert.Equal(12345, recipe.VersionId);
            recipeSerializer.FillRecipeWithHeaderInfo(recipe, @"D:\Users\MISO\Desktop\2upz.xp0");
        }

    }
}