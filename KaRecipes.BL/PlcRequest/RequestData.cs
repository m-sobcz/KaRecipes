using KaRecipes.BL.PlcRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PlcRequest
{
    public class RequestData : DataNode
    {
        public IRequest ParentRequest { get; }
    }
}
