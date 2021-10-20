using KaRecipes.BL.PartData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartData
{
    public class RequestData : DataNode
    {
        public IRequest ParentRequest { get; }
    }
}
