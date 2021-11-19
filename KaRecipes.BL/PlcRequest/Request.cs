using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PlcRequest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaRecipes.BL.PlcRequest
{
    public abstract class Request : IRequest
    {
        public Dictionary<string, RequestData> Data { get; set; }
        public RequestData Command { get; set; }
        public RequestData Acknowedgle { get; set; }
        public RequestData Error { get; set; }
        public RequestData TargetId { get; set; }

        protected IPlcDataAccess plcDataAccess;
        public Request(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public async Task Start()
        {
            bool executed = await Execute();
            if (executed)
            {
                await plcDataAccess.WriteDataNodes(Acknowedgle.NodeId, true);
            }
            else
            {
                await plcDataAccess.WriteDataNodes(Error.NodeId, true);
            }
        }
        public async Task Stop()
        {
            await plcDataAccess.WriteDataNodes(Acknowedgle.NodeId, false);
            await plcDataAccess.WriteDataNodes(Error.NodeId, false);
        }
        public abstract Task<bool> Execute();
    }
}
