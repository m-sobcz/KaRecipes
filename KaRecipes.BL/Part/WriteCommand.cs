using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Data.RequestAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    class WriteCommand : IRequestCommand
    {
        IDbWrite<PartData> dbDataAccess;
        string module;
        string station;
        public WriteCommand(IDbWrite<PartData> dbDataAccess, string module, string station) 
        {
            this.dbDataAccess = dbDataAccess;
            this.module = module;
            this.station = station;
        }
        
        public async Task<PartData> Execute(PartData stationData)
        {   
            var dataAdded = await dbDataAccess.Write(stationData);
            return dataAdded;
        }

    }
}
