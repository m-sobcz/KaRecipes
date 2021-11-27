using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.MachineStateAggregate
{
    public class MachineStateData : DataNode
    {
        public string Module { get; set; }
        public string Station { get; set; }
    }
}
