using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    public class Observer : IObserver
    {
        public void Update(PlcDataReceivedEventArgs subject)
        {
            Console.WriteLine(subject.Name + ": " + subject.Value);
        }
    }
}
