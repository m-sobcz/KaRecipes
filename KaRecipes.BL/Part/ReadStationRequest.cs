using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    class ReadStationRequest : Request
    {
        IDbReadStation<StationData> dbDataAccess;
        string module;
        string station;
        public ReadStationRequest(IDbReadStation<StationData> dbDataAccess, IPlcDataAccess plcDataAccess, string module, string station) : base(plcDataAccess)
        {
            this.dbDataAccess = dbDataAccess;
            this.module = module;
            this.station = station;
        }
        public override async Task<bool> Execute()
        {
            StationData dataReceived = await dbDataAccess.Read(PartId,module,station);
            foreach (var item in dataReceived.DataNodes)
            {
                Data.Values.FirstOrDefault(x => x.Name == item.Name).Value = item.Value;
            }
            await plcDataAccess.WriteDataNodes(dataReceived.DataNodes);
            return dataReceived.DataNodes.Count > 0;
        }
    }
}
