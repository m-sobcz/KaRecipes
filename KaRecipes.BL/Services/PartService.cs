using KaRecipes.BL.Data.RequestAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.Part;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class PartService : IObserver
    {
        Dictionary<string, RequestData> dataNodes;
        Dictionary<string, RequestData> commands;
        IPlcDataAccess plcDataAccess;
        public int PublishingInterval => 1000;

        public PartService(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }

        public void Start(List<IRequest> requests)
        {
            dataNodes = new();
            commands = new();
            foreach (var item in requests)
            {
                foreach (var readData in item.Data)
                {
                    dataNodes.Add(readData.Key, readData.Value);
                }
                dataNodes.TryAdd(item.PartId.NodeId, item.PartId);
                dataNodes.Add(item.Command.NodeId, item.Command);
                commands.Add(item.Command.NodeId, item.Command);
            }
            List<string> monitoredNodeIdentifiers = dataNodes.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(monitoredNodeIdentifiers, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            if (dataNodes.TryGetValue(subject.Name, out RequestData dataNode) == false)
            {
                throw new ArgumentException("Received unknown subject name: " + subject.Name);
            }      
            if (commands.TryGetValue(subject.Name, out RequestData commandNode))
            {
                HandleCommand(commandNode, subject.Value as bool?, dataNode.Value as bool?);
            }
            dataNode.Value = subject.Value;
        }
        void HandleCommand(RequestData commandNode, bool? actual, bool? previous) 
        {
            if (actual==true && actual!=previous)
            {
                //commandNode.ParentRequest.TargetId = dataNodes.GetValueOrDefault(commandNode.ParentRequest.TargetId.NodeId);
                commandNode.ParentRequest.ExecuteStart();
            }
            if (actual == false && actual != previous)
            {
                commandNode.ParentRequest.ExecuteStop();
            }
        }
    }
}
