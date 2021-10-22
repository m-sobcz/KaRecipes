using KaRecipes.BL.Interfaces;
using KaRecipes.BL.PlcRequest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class PartDataService : IObserver
    {
        Dictionary<string, IRequest> requests;
        Dictionary<string, RequestData> dataNodes;
        Dictionary<string, RequestData> commands;
        IPlcDataAccess plcDataAccess;
        public int PublishingInterval => 1000;

        public PartDataService(IPlcDataAccess plcDataAccess, Dictionary<string, IRequest> requests)
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
                dataNodes.TryAdd(item.Value.TargetId.NodeId, item.Value.TargetId);
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
                commandNode.ParentRequest.Command = commandNode;
                if (subject.Value.Equals(true) && dataNode.Value!=subject.Value)
                {
                    commandNode.ParentRequest.TargetId = dataNodes.GetValueOrDefault(commandNode.ParentRequest.TargetId.NodeId);
                    commandNode.ParentRequest.Start();
                }
                if (subject.Value.Equals(false) && dataNode.Value != subject.Value)
                {
                    commandNode.ParentRequest.Stop();
                }
            }
            dataNode.Value = subject.Value;
        }
    }
}
