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
        public async Task ExecuteStart()
        {
            bool executed = await Execute();
            if (executed)
            {
                Acknowedgle.Value = true;
                await plcDataAccess.WriteDataNode(Acknowedgle);
            }
            else
            {
                Error.Value = true;
                await plcDataAccess.WriteDataNode(Error);
            }
        }
        public async Task ExecuteStop()
        {
            Acknowedgle.Value = false;
            await plcDataAccess.WriteDataNode(Acknowedgle);
            Error.Value = false;
            await plcDataAccess.WriteDataNode(Error);
        }
        public abstract Task<bool> Execute();
    }
}
