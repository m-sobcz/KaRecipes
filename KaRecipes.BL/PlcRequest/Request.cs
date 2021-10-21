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
        public abstract Task<bool> Execute();
    }
}
