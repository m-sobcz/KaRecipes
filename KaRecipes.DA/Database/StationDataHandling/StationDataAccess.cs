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
    public class StationDataAccess : IDbWrite<PartData>
    {
        ISqlDataAccess sqlDataAccess;
        public StationDataAccess(ISqlDataAccess sqlDataAccess)
        {
            this.sqlDataAccess = sqlDataAccess;
        }

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
