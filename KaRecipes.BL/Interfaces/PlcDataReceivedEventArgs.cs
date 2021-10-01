using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public class PlcDataReceivedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public object Value { get; set; }

    }
}
