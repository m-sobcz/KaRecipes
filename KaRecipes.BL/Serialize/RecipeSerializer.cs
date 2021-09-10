using KaRecipes.BL.RecipeAggregate;
using KaRecipes.BL.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace KaRecipes.BL.Serialize
{
    public class RecipeSerializer : IRecipeSerializer
    {
        readonly string groupNameAttribute = "name";
        readonly string parameterNameAttribute = "name";
        readonly string parameterValueAttribute = "value";
        public Recipe Deserialize(string text)
        {
            var root = XElement.Parse(text);
            var loadedModules = root.Elements().Elements();
            Recipe recipe = new();
            foreach (var loadedModule in loadedModules)
            {
                ParameterModule newParameterModule = LoadParameterModule(loadedModule);
                recipe.ParameterModules.Add(newParameterModule);
            }
            return recipe;
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
        public void FillRecipeWithHeaderInfo(Recipe recipe, string headerInfo)
        {
            Regex regex = new(@".+\\([^_,\.]+)_?([^\.]+)?");
            var match = regex.Match(headerInfo);
            if (match.Success)
            {
                recipe.Name = match.Groups[1].Value;
                if (int.TryParse(match.Groups[2].Value, out int versionId))
                {
                    recipe.VersionId = versionId;
                }
            }
        }
        public string Serialize(Recipe recipe)
        {
            XElement xParameterGroups = new("ParameterGroups");
            foreach (var module in recipe.ParameterModules)
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
            StringBuilder stringBuilder = new();
            using (XmlWriter xw = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                xElement.Save(xw);
            }
            return stringBuilder.ToString();
        }
    }
}
