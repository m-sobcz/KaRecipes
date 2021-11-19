using KaRecipes.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL
{
    public static class Traceability
    {
        public static void Notify(PlcDataReceivedEventArgs subject, [CallerMemberName] string callerName = "")
        {
            Trace.WriteLine($"Caller: {callerName} unable to handle data: {subject.Name} with value: {subject.Value}");
        }
    }
}
