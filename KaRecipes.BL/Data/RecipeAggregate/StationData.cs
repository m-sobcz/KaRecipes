using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.RecipeAggregate
{
    public class StationData
    {
        public List<SingleParamData> Params { get; set; } = new();
        public string Name { get; set; }

        public ModuleData Module { get; set; }

        public StationData(ModuleData Module, string name)
        {
            this.Module = Module;
            this.Name = name;
        }
        public SingleParamData AddParam(string name, string nodeId, object value = null)
        {
            SingleParamData singleParamData = new(this,name,nodeId,value);
            Params.Add(singleParamData);
            return singleParamData;
        }
    }
}
