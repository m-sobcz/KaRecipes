using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PartAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PlcRequest
{
    class ReadRequest : Request
    {
        IDbDataAccess<PartData> dbDataAccess;
        string module;
        string station;
        public ReadRequest(IDbDataAccess<PartData> dbDataAccess, IPlcDataAccess plcDataAccess, string module, string station) : base(plcDataAccess)
        {
            this.dbDataAccess = dbDataAccess;
            this.module = module;
            this.station = station;
        }
        public override async Task<bool> Execute()
        {
            var dataReceived = await dbDataAccess.Read(TargetId);
            foreach (var item in dataReceived.DataNodes)
            {
                Data.Values.Where(x => x.Name == item.Name).FirstOrDefault().Value = item.Value;
            }
            foreach (var item in Data.Values)
            {
                await plcDataAccess.WriteDataNodes(item.NodeId, item.Value);
            }
            return dataReceived.DataNodes.Count > 0;
        }
    }
}
