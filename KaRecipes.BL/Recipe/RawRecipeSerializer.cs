using KaRecipes.BL.Data.RecipeAggregate;
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

namespace KaRecipes.BL.Recipe
{
    public class RawRecipeSerializer : IRawRecipeSerializer
    {
        readonly string groupNameAttribute = "name";
        readonly string parameterNameAttribute = "name";
        readonly string parameterValueAttribute = "value";

        public RawRecipeData Deserialize(string text)
        {
            var root = XElement.Parse(text);
            var loadedModules = root.Elements().Elements();
            RawRecipeData recipe = new();
            foreach (var loadedModule in loadedModules)
            {
                ModuleData newParameterModule = LoadParameterModule(loadedModule);
                recipe.Modules.Add(newParameterModule);
            }
            return recipe;
        }
        ModuleData LoadParameterModule(XElement loadedModule)
        {
            ModuleData newParameterModule = new(loadedModule.Attribute(groupNameAttribute).Value);
            foreach (var loadedStation in loadedModule.Elements())
            {
                StationData newParameterStation = LoadParameterStation(loadedStation, newParameterModule);
                newParameterModule.Stations.Add(newParameterStation);
            }
            return newParameterModule;
        }
        StationData LoadParameterStation(XElement loadedStation, ModuleData moduleData)
        {
            StationData newParameterStation = new(moduleData, loadedStation.Attribute(groupNameAttribute).Value);
            foreach (var loadedParameter in loadedStation.Elements())
            {
                var name = loadedParameter.Attribute(parameterNameAttribute).Value;
                var value = loadedParameter.Attribute(parameterValueAttribute).Value;
                newParameterStation.Params.Add(new SingleParamData(newParameterStation,name,null) {Value = value });
            }
            return newParameterStation;
        }
        public void FillRecipeWithHeaderInfo(RawRecipeData recipe, string headerInfo)
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
        public string Serialize(RecipeData recipe)
        {
            XElement xParameterGroups = new("ParameterGroups");
            foreach (var module in recipe.Modules)
            {
                XElement xModule = new("ParameterGroup");
                xModule.SetAttributeValue("name", module.Name);
                foreach (var station in module.Stations)
                {
                    XElement xStation = new("ParameterGroup");
                    station.Name = module.Name + "_" + station.Name;
                    xStation.SetAttributeValue("name", station.Name);
                    foreach (var singleParam in station.Params)
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
            //  SaveXElementToFile(xRoot, path);
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
