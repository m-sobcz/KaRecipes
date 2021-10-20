using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartData
{
    public class WriteRequest : IRequest
    {
        public ConcurrentDictionary<string, RequestData> Data { get; set; }
        public RequestData Command { get; set; }
        public RequestData Acknowedgle { get; set; }
        public RequestData Error { get; set; }
        IDbDataAccess<Dictionary<string, object>> dbDataAccess;
        IPlcDataAccess plcDataAccess;
        public WriteRequest(IDbDataAccess<Dictionary<string, object>> dbDataAccess, IPlcDataAccess plcDataAccess)
        {
            this.dbDataAccess = dbDataAccess;
            this.plcDataAccess = plcDataAccess;
        }
        public async Task Start()
        {
            Dictionary<string, object> transferData = new();
            foreach (var item in Data)
            {
                transferData.Add(item.Value.Name, item.Value.Value);
            }
            int? dataAdded = await dbDataAccess.Write(transferData);
            if (dataAdded.HasValue && dataAdded.Value > 0)
            {
                await plcDataAccess.WriteParameter(Acknowedgle.NodeId, true);
            }
            else
            {
                await plcDataAccess.WriteParameter(Error.NodeId, true);
            }
        }
        public async Task Stop()
        {
            await plcDataAccess.WriteParameter(Acknowedgle.NodeId, false);
            await plcDataAccess.WriteParameter(Error.NodeId, false);
        }
    }
}
