using KaRecipes.BL.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IPlcDataAccess
    {
        event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;

        Task CreateSubscriptionsWithInterval(List<string> monitoredNodeIdentifiers, int publishingInterval, IObserver observer);
        
        Task<DataNode> ReadDataNode(string nodeIdentifier);

        Task Start();

        Task<bool> WriteDataNodes(List<DataNode> dataNodes);
        Task<bool> WriteDataNode(DataNode dataNode);

        void Dispose();

        string PlcAccessPrefix { get; }
        Dictionary<string, string> GetAvailableNodes();
    }
}