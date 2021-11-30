using KaRecipes.BL.Data;
using KaRecipes.BL.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.RequestAggregate
{
    public class RequestData : DataNode
    {
        public IRequest ParentRequest { get; set; }
    }
}
