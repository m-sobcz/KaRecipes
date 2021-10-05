using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IAvailableNodesSource
    {
        Dictionary<string, string> AvailableNodes { get; set; }
    }
}
