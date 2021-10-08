using KaRecipes.BL.AlarmAggregate;
using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Services
{
    public class AlarmsService : IObserver
    {
        IPlcDataAccess plcDataAccess;
        public int PublishingInterval => 1000;

        public AlarmsService(IPlcDataAccess plcDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
        }

        public void Subscribe(Dictionary<string, Alarm> alarms)
        {
            List<string> keys = alarms.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(keys, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            throw new NotImplementedException();
        }
    }
}
