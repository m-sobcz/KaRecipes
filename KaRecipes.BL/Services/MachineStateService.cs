using KaRecipes.BL.Interfaces;
using KaRecipes.BL.MachineStateAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class MachineStateService : IObserver
    {
        Dictionary<string, MachineState> machineStates;
        IPlcDataAccess plcDataAccess;
        IDbDataAccess<MachineState> dbDataAccess;
        public MachineStateService(IPlcDataAccess plcDataAccess, IDbDataAccess<MachineState> dbDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
            this.dbDataAccess = dbDataAccess;
        }

        public int PublishingInterval => 5000;
        public void Start(Dictionary<string, MachineState> machineStates)
        {
            this.machineStates = machineStates;
            List<string> keys = machineStates.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(keys, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            machineStates.TryGetValue(subject.Name, out MachineState machineState);
            dbDataAccess.Write(machineState);
        }

    }
}
