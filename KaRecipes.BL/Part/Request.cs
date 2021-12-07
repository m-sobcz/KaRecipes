using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Data.RequestAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KaRecipes.BL.Part
{
    public class Request : IRequest
    {
        public PartData Data { get; set; }
        public RequestData Command { get; set; }
        public RequestData Acknowedgle { get; set; }
        public RequestData Error { get; set; }
        public RequestData PartId { get; set; }
        protected IRequestCommand command; 
        protected IPlcDataAccess plcDataAccess;
        public Request(IPlcDataAccess plcDataAccess, IRequestCommand command)
        {
            this.plcDataAccess = plcDataAccess;
            this.command = command;
        }
        public async Task ExecuteStart(PartData stationData)
        {
            var returnedObject = await command.Execute(stationData);
            if (returnedObject != null)
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

    }
}
