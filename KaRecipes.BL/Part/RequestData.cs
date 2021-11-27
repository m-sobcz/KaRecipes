using KaRecipes.BL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    public class RequestData : DataNode
    {
        public IRequest ParentRequest { get; set; }
    }
}
