using KaRecipes.BL.Data;
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
    class ReadStationCommand : IRequestCommand
    {
        IDbReadStation<PartData> dbDataAccess;
        IPlcDataAccess plcDataAccess;

        public ReadStationCommand(IDbReadStation<PartData> dbDataAccess, IPlcDataAccess plcDataAccess) 
        {
            this.dbDataAccess = dbDataAccess;
            this.plcDataAccess = plcDataAccess;
        }
        public async Task<PartData> Execute(PartData stationData)
        {
            PartData dataReceived = await dbDataAccess.Read(stationData);
            List<DataNode> dataNodesToWrite = new(dataReceived.DataNodes);
            await plcDataAccess.WriteDataNodes(dataNodesToWrite);
            return dataReceived;
        }
    }
}
