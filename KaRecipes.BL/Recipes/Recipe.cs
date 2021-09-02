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
        public Recipe()
        {
        }
        public List<ParameterModule> LoadXml(string path)
        {
            var root = XElement.Load(path);
            var loadedModules = root.Elements().Elements();
            List<ParameterModule> newParameterModules = new();
            foreach (var loadedModule in loadedModules)
            {
                ParameterModule newParameterModule = LoadParameterModule(loadedModule);
                newParameterModules.Add(newParameterModule);
            }
            return newParameterModules;
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
    }
}
