using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PartAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.Logic
{
    class PartDataProcessor : IDbDataAccess<PartData>
    {
        public Task<PartData> Read(object id)
        {
            throw new NotImplementedException();
        }

        public Task<int?> Write(PartData data)
        {
            throw new NotImplementedException();
        }
    }
}
