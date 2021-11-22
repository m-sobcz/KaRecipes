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
        IDbDataAccess<Alarm> dbDataAccess;
        public Dictionary<string, Alarm> Alarms { get; private set; }
        public int PublishingInterval => 1000;

        public AlarmsService(IPlcDataAccess plcDataAccess, IDbDataAccess<Alarm> dbDataAccess)
        {
            this.plcDataAccess = plcDataAccess;
            this.dbDataAccess = dbDataAccess;
        }

        public void Start(Dictionary<string, Alarm> alarms)
        {
            this.Alarms = alarms;
            List<string> keys = alarms.Keys.ToList();
            plcDataAccess.CreateSubscriptionsWithInterval(keys, PublishingInterval, this);
        }

        public void Update(PlcDataReceivedEventArgs subject)
        {
            if (Alarms.TryGetValue(subject.Name, out Alarm alarm))
            {
                var comparableAlarm = alarm.Value as IComparable;
                bool valueChanged = comparableAlarm?.CompareTo(subject.Value)!=0;
                alarm.Value = subject.Value;
                if (valueChanged) dbDataAccess.Write(alarm);
            }
            else
            {
                throw new ArgumentException("Received unknown subject name: " + subject.Name);
                //Traceability.Notify(subject);
            }
        }
    }
}
