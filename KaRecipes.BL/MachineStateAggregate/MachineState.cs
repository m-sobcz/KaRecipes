using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.MachineStateAggregate
{
    public class MachineState
    {
        public DataNode State { get; set; }
        public string Module { get; set; }
        public string Station { get; set; }
    }
}
