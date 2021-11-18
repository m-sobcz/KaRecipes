using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class LifeBitService 
    {
        public CancellationTokenSource StopNotifyingTokenSource { get; private set; }
        IPlcDataAccess plcDataAccess;
        Dictionary<string, DataNode> dataNodes;
        int Interval => 1000;
        public LifeBitService(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public void Start(Dictionary<string,DataNode> dataNodes) 
        {
            this.dataNodes = dataNodes;
            StopNotifyingTokenSource = new();
            Task.Run(() => Notify(StopNotifyingTokenSource.Token));
        }
        async Task Notify(CancellationToken cancellationToken) 
        {
            while (cancellationToken.IsCancellationRequested == false) 
            {
                foreach (var item in dataNodes.Values)
                {
                    var boolVal = item.Value as bool?;
                    item.Value = boolVal.HasValue && !boolVal.Value;
                    await plcDataAccess.WriteParameter(item.NodeId, item.Value);
                }
                await Task.Delay(Interval, cancellationToken);
            }
        }

    }
}
