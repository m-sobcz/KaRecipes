using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.DA.OPC
{
    public class OpcDataReceivedEventArgs : EventArgs
    {
        public string Name { get; set; }
        public object Value { get; set; }

    }
}
