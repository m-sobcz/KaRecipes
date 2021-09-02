using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.ParameterModuleAggregate
{
    public class ParameterStation
    {
        public List<ParameterSingle> ParameterSingles = new();
        public string Name { get; set; }
    }
}
