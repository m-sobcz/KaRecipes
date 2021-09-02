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
        public void LoadXmlTest()
        {
            string testPath = AppContext.BaseDirectory + @"\TestData\SAG2_Actuator.xml";
            var actual = new Recipe().LoadXml(testPath);
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
            var comparison=compareLogic.Compare(expected, actual);
            Assert.True(comparison.AreEqual);
        }
    }
}