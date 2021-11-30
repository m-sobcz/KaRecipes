using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    class WriteRequest : Request
    {
        IDbWrite<StationData> dbDataAccess;
        string module;
        string station;
        public WriteRequest(IDbWrite<StationData> dbDataAccess, IPlcDataAccess plcDataAccess, string module, string station) : base(plcDataAccess) 
        {
            this.dbDataAccess = dbDataAccess;
            this.module = module;
            this.station = station;
        }
        public override async Task<bool> Execute()
        {   
            var partDataFields= Data.Select(x=>x.Value);
            StationData partData = new() { Module = module, Station = station, DataNodes = new(partDataFields) };
            int? dataAdded = await dbDataAccess.Write(partData);
            return dataAdded > 0;
        }
    }
}
