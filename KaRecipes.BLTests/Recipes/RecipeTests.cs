using Xunit;
using KaRecipes.BL.Recipes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using KaRecipes.BL.ParameterModuleAggregate;
using KellermanSoftware.CompareNetObjects;

namespace KaRecipes.BL.Recipes.Tests
{
    public class RecipeTests
    {
        [Fact()]
        public void LoadXml_SampleData_AreEqualAndReturnsCount()
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
            Recipe recipe = new();
            var result = recipe.LoadXml(xml);
            var expected = new List<ParameterModule>
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
            };
            CompareLogic compareLogic = new();
            var comparison = compareLogic.Compare(expected, recipe.productionLineParameters);
            Assert.Equal(2, result);
            Assert.True(comparison.AreEqual);
        }
        [Fact()]
        public void LoadXml_Empty_ReturnsZero()
        {
            string xml =
@"<Parameters>
  <ParameterGroups>
  </ParameterGroups>
</Parameters>";
            Recipe recipe = new();
            var result=recipe.LoadXml(xml);
            Assert.Equal(0, result);
        }
        [Fact()]
        public void GenerateXml_SampleData()
        {
            List<ParameterStation> stations1 = new();
            ParameterSingle single11 = new() { Name = "single11", Value = "11" };
            ParameterSingle single12 = new() { Name = "single12", Value = "12" };
            List<ParameterSingle> singles1 = new();
            singles1.Add(single11);
            singles1.Add(single12);
            stations1.Add(new() { Name = "singles1", ParameterSingles = singles1});

            List<ParameterStation> stations2 = new();
            ParameterSingle single21 = new() { Name = "single21", Value = "21" };
            ParameterSingle single22 = new() { Name = "single22", Value = "22" };
            List<ParameterSingle> singles2 = new();
            singles2.Add(single21);
            singles2.Add(single22);
            stations2.Add(new() { Name = "singles2", ParameterSingles = singles2 });

            Recipe recipe = new();
            recipe.productionLineParameters = new();
            recipe.productionLineParameters.Add(new ParameterModule() { Name = "module1", ParameterStations= stations1 });
            recipe.productionLineParameters.Add(new ParameterModule() { Name = "module2", ParameterStations = stations2 });
            string actual = recipe.GenerateXml();

            string expected = 
@"<Parameters>
  <ParameterGroups>
    <ParameterGroup name=""module1"">
      <ParameterGroup name=""singles1"">
        <Parameter name=""single11"" value=""11"" />
        <Parameter name=""single12"" value=""12"" />
      </ParameterGroup>
    </ParameterGroup>
    <ParameterGroup name=""module2"">
      <ParameterGroup name=""singles2"">
        <Parameter name=""single21"" value=""21"" />
        <Parameter name=""single22"" value=""22"" />
      </ParameterGroup>
    </ParameterGroup>
  </ParameterGroups>
</Parameters>";
            Assert.Equal(expected, actual);
        }
    }
}