﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IObserver
    {
        public void Update(PlcDataReceivedEventArgs subject);
        public int PublishingInterval { get;}
    }
}
