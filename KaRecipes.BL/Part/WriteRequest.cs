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
        IDbDataAccess<PartData> dbDataAccess;
        string module;
        string station;
        public WriteRequest(IDbDataAccess<PartData> dbDataAccess, IPlcDataAccess plcDataAccess, string module, string station) : base(plcDataAccess) 
        {
            this.dbDataAccess = dbDataAccess;
            this.module = module;
            this.station = station;
        }
        public override async Task<bool> Execute()
        {
            var requestedData= Data.Select(x=>x.Value);
            PartData partData = new() { Module = module, Station = station, DataNodes = new(requestedData) };
            int? dataAdded = await dbDataAccess.Write(partData);
            return dataAdded > 0;
        }
    }
}
