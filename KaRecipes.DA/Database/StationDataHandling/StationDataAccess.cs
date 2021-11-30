using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.DA.Database.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.Database.StationDataHandling
{
    public class StationDataAccess : IDbWrite<StationData>
    {
        ISqlDataAccess sqlDataAccess;
        public StationDataAccess(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }

        public Task<StationData> Read(object id, string module, string station)
        {
            throw new NotImplementedException();
        }

        public Task<int?> Write(StationData data)
        {
            throw new NotImplementedException();
        }
    }
}
