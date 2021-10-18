using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartData
{
    public class PlcRequestHandler : IObserver
    {
        Dictionary<string, DataNode> dataNodes;
        Dictionary<string, Action> commands;
        IPlcDataAccess plcDataAccess;
        public int PublishingInterval => 1000;
        ReadRequestData readRequestData;


        public PlcRequestHandler(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }

        public void Start(Dictionary<string, DataNode> dataNodes)
        {
            this.dataNodes = dataNodes;
            List<string> keys = dataNodes.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(keys, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            dataNodes.TryGetValue(subject.Name, out DataNode dataNode);
            //Wykrywanie zbocza na sygnale komendy
            if (commands.TryGetValue(dataNode.Name, out Action commandAction) && subject.Value.Equals(true) && dataNode.Value.Equals(false))
            {
                commandAction();
            }
            dataNode.Value = subject.Value;
        }
    }
}
