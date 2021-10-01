using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IPlcDataAccess
    {
        event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;

        void CreateSubscriptions(List<string> monitoredNodeIdentifiers);
        void Dispose();
        object ReadNode(string nodeIdentifier);
        Task Start();
        void WriteToNode(string nodeIdentifier, object value);
    }
}