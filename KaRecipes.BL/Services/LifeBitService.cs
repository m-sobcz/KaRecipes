using KaRecipes.BL.Data;
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
        List<DataNode> dataNodes;
        public int Interval { get; set; } = 1000;
        public LifeBitService(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }
        public void Start(List<DataNode> dataNodes) 
        {
            this.dataNodes = dataNodes;
            StopNotifyingTokenSource = new();
            Task.Run(() => Notify(StopNotifyingTokenSource.Token));
        }
        public void Stop() 
        {
            StopNotifyingTokenSource.Cancel();
        }
        async Task Notify(CancellationToken cancellationToken) 
        {
            while (cancellationToken.IsCancellationRequested == false) 
            {
                foreach (var item in dataNodes)
                {
                    var boolVal = item.Value as bool?;
                    item.Value = boolVal.HasValue && !boolVal.Value;
                }
                await plcDataAccess.WriteDataNodes(dataNodes);
                await Task.Delay(Interval, cancellationToken);
            }
        }

    }
}
