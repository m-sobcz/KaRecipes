using KaRecipes.BL.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    public class Observer : IObserver
    {
        public int PublishingInterval => 1000;

        public void Update(PlcDataReceivedEventArgs subject)
        {
            Console.WriteLine(subject.Name + ": " + subject.Value);
            if (subject.Value.GetType().IsGenericType)
            {
                foreach (var item in subject.Value as IEnumerable)
                {
                    Console.Write(": " + item);
                }
                Console.WriteLine();
            }
        }
    }
}
