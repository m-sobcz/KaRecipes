using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace CookBookBLL.Logic
{
    public class Processor
    {
        protected string defaultStoredProceduresPrefix;
        protected string GetDefaultsql([CallerMemberName] string callerMemberName = "")
        {
            return $"{defaultStoredProceduresPrefix}_{callerMemberName}";
        }
    }
}
