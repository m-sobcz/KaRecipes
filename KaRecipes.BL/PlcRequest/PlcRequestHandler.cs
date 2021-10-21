using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PlcRequest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PlcRequest
{
    public class PlcRequestHandler : IObserver
    {
        Dictionary<string, IRequest> requests;
        Dictionary<string, RequestData> dataNodes;
        Dictionary<string, RequestData> commands;
        IPlcDataAccess plcDataAccess;
        public int PublishingInterval => 1000;

        public PlcRequestHandler(IPlcDataAccess plcDataAccess, Dictionary<string, IRequest> requests)
        {
            this.requests = requests;
            this.plcDataAccess = plcDataAccess;
        }

        public void Start()
        {
            dataNodes = new();
            commands = new();
            foreach (var item in requests)
            {         
                foreach (var readData in item.Value.Data)
                {
                    dataNodes.Add(readData.Key, readData.Value);
                }
                dataNodes.Add(item.Value.Command.NodeId, item.Value.Command);
                commands.Add(item.Value.Command.NodeId, item.Value.Command);
            }
            List<string> monitoredNodeIdentifiers = dataNodes.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(monitoredNodeIdentifiers, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            dataNodes.TryGetValue(subject.Name, out RequestData dataNode);
            if (commands.TryGetValue(subject.Name, out RequestData commandNode))
            {
                if (subject.Value.Equals(true) && dataNode.Value.Equals(false)) 
                { 
                    commandNode.ParentRequest.Start();
                }
                if (subject.Value.Equals(false) && dataNode.Value.Equals(true))
                {
                    commandNode.ParentRequest.Stop();
                }
            }    
            dataNode.Value = subject.Value;
        }
    }
}
