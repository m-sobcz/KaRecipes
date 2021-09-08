using KaRecipes.BL.ParameterModuleAggregate;
using KaRecipes.BL.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KaRecipes.BL.Recipes
{
    public class Recipe
    {
        readonly string groupNameAttribute = "name";
        readonly string parameterNameAttribute = "name";
        readonly string parameterValueAttribute = "value";
        public List<ParameterModule> productionLineParameters { get; set; }
        public Recipe()
        {
        }
        public int LoadXml(string text)
        {
            var root = XElement.Parse(text);
            var loadedModules = root.Elements().Elements();
            productionLineParameters = new();
            foreach (var loadedModule in loadedModules)
            {
                ParameterModule newParameterModule = LoadParameterModule(loadedModule);
                productionLineParameters.Add(newParameterModule);
            }
            return productionLineParameters.Count;
        }
        ParameterModule LoadParameterModule(XElement loadedModule)
        {
            ParameterModule newParameterModule = new()
            {
                Name = loadedModule.Attribute(groupNameAttribute).Value
            };
            foreach (var loadedStation in loadedModule.Elements())
            {
                ParameterStation newParameterStation = LoadParameterStation(loadedStation);
                newParameterModule.ParameterStations.Add(newParameterStation);
            }
            return newParameterModule;
        }
        ParameterStation LoadParameterStation(XElement loadedStation)
        {
            ParameterStation newParameterStation = new()
            {
                Name = loadedStation.Attribute(groupNameAttribute).Value
            };
            foreach (var loadedParameter in loadedStation.Elements())
            {
                var name = loadedParameter.Attribute(parameterNameAttribute).Value;
                var value = loadedParameter.Attribute(parameterValueAttribute).Value;
                ParameterSingle newParameterSingle = new() { Name = name, Value = value };
                newParameterStation.ParameterSingles.Add(newParameterSingle);
            }
            return newParameterStation;
        }
        public string GenerateXml()
        {
            XElement xParameterGroups = new("ParameterGroups");
            foreach (var module in productionLineParameters)
            {
                XElement xModule = new("ParameterGroup");
                xModule.SetAttributeValue("name", module.Name);
                foreach (var station in module.ParameterStations)
                {
                    XElement xStation = new("ParameterGroup");
                    xStation.SetAttributeValue("name", station.Name);
                    foreach (var singleParam in station.ParameterSingles)
                    {
                        XElement xSingleParam = new("Parameter");
                        xSingleParam.SetAttributeValue("name", singleParam.Name);
                        xSingleParam.SetAttributeValue("value", singleParam.Value);
                        xStation.Add(xSingleParam);
                    }
                    xModule.Add(xStation);
                }
                xParameterGroups.Add(xModule);
            }
            XElement xRoot = new("Parameters", xParameterGroups);
            return GenerateFormattedXml(xRoot);
            // SaveXElementToFile(xRoot, path);
        }
        string GenerateFormattedXml(XElement xElement)
        {
            XmlWriterSettings xmlWriterSettings = new();
            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.Indent = true;
            // xmlWriterSettings.NewLineOnAttributes = true;
            StringBuilder stringBuilder = new();
            using (XmlWriter xw = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                xElement.Save(xw);
            }
            return stringBuilder.ToString();
        }
    }
}
