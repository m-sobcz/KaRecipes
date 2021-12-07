using KaRecipes.BL.Data.RequestAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Data.PartAggregate
{
    public class PartData
    {
        public List<RequestData> DataNodes { get; set; }
        public int? Id { get; set; }
        public string Module { get; set; }
        public string Station { get; set; }
    }
}
