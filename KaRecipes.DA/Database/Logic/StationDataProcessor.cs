using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.Logic
{
    class StationDataProcessor : IDbWrite<PartData>
    {

        public Task<PartData> Read(object id, string module, string station)
        {
            throw new NotImplementedException();
        }

        public Task<int?> Write(PartData data)
        {
            throw new NotImplementedException();
        }

        Task<PartData> IDbWrite<PartData>.Write(PartData data)
        {
            throw new NotImplementedException();
        }
    }
}
