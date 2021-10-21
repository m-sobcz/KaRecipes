using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartAggregate
{
    public class PartData
    {
        public List<DataNode> DataNodes { get; set; }
        public string Module { get; set; }
        public string Station { get; set; }
    }
}
