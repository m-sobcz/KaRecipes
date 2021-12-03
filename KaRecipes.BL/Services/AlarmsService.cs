using KaRecipes.BL.Data.AlarmAggregate;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.Utils;
using KellermanSoftware.CompareNetObjects;
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
        IDbWrite<AlarmData> dbDataAccess;
        CompareLogic compare = new();
        public Dictionary<string, AlarmData> Alarms { get; private set; }
        public int PublishingInterval => 1000;

        public AlarmsService(IPlcDataAccess plcDataAccess, IDbWrite<AlarmData> dbDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
            this.dbDataAccess = dbDataAccess;
        }

        public void Start(Dictionary<string, AlarmData> alarms)
        {
            this.Alarms = alarms;
            List<string> keys = alarms.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(keys, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            if (Alarms.TryGetValue(subject.Name, out AlarmData alarm))
            {
                bool valueChanged = compare.Compare(alarm.Value, subject.Value).AreEqual == false;
                alarm.Value = subject.Value;
                if (valueChanged) dbDataAccess.Write(alarm);
            }
            else
            {
                Traceability.Notify(subject);
            }
        }
    }
}
