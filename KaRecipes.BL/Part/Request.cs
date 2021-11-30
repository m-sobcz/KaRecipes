using KaRecipes.BL.Data.RequestAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    public abstract class Request : IRequest
    {
        public Dictionary<string, RequestData> Data { get; set; }
        public RequestData Command { get; set; }
        public RequestData Acknowedgle { get; set; }
        public RequestData Error { get; set; }
        public RequestData PartId { get; set; }

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
                await SetRequest(Acknowedgle);
            }
            else
            {
                await SetRequest(Error);
            }
        }
        public async Task ExecuteStop()
        {
            await ResetRequest(Acknowedgle);
            await ResetRequest(Error);
        }
        protected async Task<bool> SetRequest(RequestData requestData) 
        {
            requestData.Value = true;
            bool success=await plcDataAccess.WriteDataNode(requestData);
            return success;
        }
        protected async Task<bool> ResetRequest(RequestData requestData)
        {
            requestData.Value = false;
            bool success = await plcDataAccess.WriteDataNode(requestData);
            return success;
        }
        public abstract Task<bool> Execute();
    }
}
