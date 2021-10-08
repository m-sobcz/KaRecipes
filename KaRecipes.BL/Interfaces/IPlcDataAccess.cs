﻿using KaRecipes.BL.RecipeAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IPlcDataAccess
    {
        event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;

        Task CreateSubscriptionsWithInterval(List<string> monitoredNodeIdentifiers, int publishingInterval, IObserver observer);
        
        Task<ParameterSingle> ReadParameter(string nodeIdentifier);

        Task Start();

        Task<bool> WriteParameter(string nodeIdentifier, object value);

        void Dispose();

        string PlcAccessPrefix { get; }

        Dictionary<string, string> GetAvailableNodes();
    }
}