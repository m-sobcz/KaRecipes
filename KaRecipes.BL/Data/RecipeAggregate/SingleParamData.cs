using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.RecipeAggregate
{
    public class SingleParamData : DataNode
    {
        public StationData Station { get;}

        public SingleParamData(StationData Station, string name, string nodeId, object value=null)
        {
            this.Station = Station;
            Name = name;
            NodeId = nodeId;
            Value = value;
        }
    }
}
