using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IPlcDataAccess
    {
        event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;

        Task CreateSubscriptions(List<string> monitoredNodeIdentifiers);
        
        Task<object> ReadNode(string nodeIdentifier);
        Task Start();
        Task WriteToNode(string nodeIdentifier, object value);
        void Dispose();
    }
}