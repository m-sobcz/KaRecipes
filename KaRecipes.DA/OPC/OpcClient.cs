using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using KaRecipes.BL;
using KaRecipes.BL.Interfaces;
using KaRecipes.BL.RecipeAggregate;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace KaRecipes.DA.OPC
{
    public class OpcClient : IDisposable, IPlcDataAccess
    {
        bool disposed = false;
        readonly ushort namespaceIndex = 2;
        readonly ApplicationInstance opcApplication;
        const int ReconnectPeriod = 10;
        readonly int discoverTimeout = 15000;
        readonly uint sessionTimeout = 60000;
        Session session;
        SessionReconnectHandler reconnectHandler;
        readonly string nodeIdPrefix = "KaRecipes";
        public event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;
        Dictionary<uint, IObserver> observers = new();

        readonly Regex nodeNameRegex = new(@"(?<=\.)\w+\b(?!\.)", RegexOptions.Compiled);

        public string PlcAccessPrefix => nodeIdPrefix;

        public ParameterSingle ParameterSingle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public OpcClient()
        {
            opcApplication = new ApplicationInstance
            {
                ApplicationName = "KaRecipes OPC UA Client",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName = "OPC"
            };
        }

        public async Task Start()
        {
            ApplicationConfiguration config = await opcApplication.LoadApplicationConfiguration(false);
            bool haveAppCertificate = await opcApplication.CheckApplicationInstanceCertificate(false, 0);
            string endpointUrl = config.ServerConfiguration.BaseAddresses.First();
            config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, haveAppCertificate, discoverTimeout);
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            session = await Session.Create(config, endpoint, false, opcApplication.ApplicationName, sessionTimeout, new UserIdentity(new AnonymousIdentityToken()), null);
            session.KeepAlive += Client_KeepAlive;
        }

        public async Task<DataNode> ReadDataNode(string nodeIdentifier)
        {
            var readVal = await ReadDataValue(nodeIdentifier);
            var convertedVal = DataValueToNetType(readVal);
            var name = ExtractNameFromIdentifier(nodeIdentifier);
            return new DataNode() { Name = name, Value = convertedVal, NodeId = nodeIdentifier }; 
        }

        string ExtractNameFromIdentifier(string nodeIdentifier)
        {
            var match = nodeNameRegex.Match(nodeIdentifier);
            var name = match.Value;
            return name;
        }

        async Task<DataValue> ReadDataValue(string nodeIdentifier)
        {
            NodeId nodeId1 = new(nodeIdentifier, 2);
            var readVal = await Task.Run(() => session.ReadValue(nodeId1));
            return readVal;
        }

        public async Task CreateSubscriptionsWithInterval(List<string> monitoredNodeIdentifiers, int publishingInterval, IObserver observer)
        {
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = publishingInterval };
            var MonitoredItems = new List<MonitoredItem>();
            foreach (var item in monitoredNodeIdentifiers)
            {
                var nodeId = new NodeId(item, 2);
                MonitoredItems.Add(
                    new MonitoredItem(subscription.DefaultItem) { DisplayName = nodeId.Identifier.ToString(), StartNodeId = nodeId });
            }
            MonitoredItems.ForEach(i => i.Notification += OnNotification);
            subscription.AddItems(MonitoredItems);
            await Task.Run(() => session.AddSubscription(subscription));
            subscription.Create();
            observers.Add(subscription.Id, observer);
        }

        public async Task<bool> WriteDataNodes(List<DataNode> dataNodes)
        {
            WriteValueCollection valuesToWrite = new WriteValueCollection();
            foreach (var node in dataNodes)
            {
                WriteValue valueToWrite = new();
                valueToWrite.NodeId = new NodeId(node.NodeId, namespaceIndex);
                valueToWrite.AttributeId = Attributes.Value;
                valueToWrite.Value.Value = node.Value;
                valueToWrite.Value.StatusCode = StatusCodes.Good;
                valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
                valueToWrite.Value.SourceTimestamp = DateTime.MinValue;
                valuesToWrite.Add(valueToWrite);
            }        
            // write current value.
            StatusCodeCollection results = null;
            DiagnosticInfoCollection diagnosticInfos = null;
            await Task.Run(() => session.Write(
                null,
                valuesToWrite,
                out results,
                out diagnosticInfos));
            ClientBase.ValidateResponse(results, valuesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);
            return StatusCode.IsGood(results[0]);
        }

        public Dictionary<string, string> GetAvailableNodes()
        {
            Dictionary<string, string> nodes = new();
            session.Browse(
                null,
                null,
                new NodeId(nodeIdPrefix, namespaceIndex),
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Object,
                out _,
                out ReferenceDescriptionCollection moduleRefs);
            foreach (var moduleRef in moduleRefs)
            {
                session.Browse(
                    null,
                    null,
                    ExpandedNodeId.ToNodeId(moduleRef.NodeId, session.NamespaceUris),
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                    out byte[] nextCp,
                    out ReferenceDescriptionCollection stationRefs);

                foreach (var stationRef in stationRefs)
                {
                    if (stationRef.DisplayName.ToString().StartsWith("_") == false)
                    {
                        session.Browse(
                        null,
                        null,
                        ExpandedNodeId.ToNodeId(stationRef.NodeId, session.NamespaceUris),
                        0u,
                        BrowseDirection.Forward,
                        ReferenceTypeIds.HierarchicalReferences,
                        true,
                        (uint)NodeClass.Variable,
                        out byte[] next2Cp,
                        out ReferenceDescriptionCollection parameterRefs);

                        foreach (var parameterRef in parameterRefs)
                        {
                            if (parameterRef.DisplayName.ToString().StartsWith("_") == false)
                            {
                                nodes.Add(parameterRef.NodeId.Identifier.ToString(), parameterRef.DisplayName.ToString());
                            }
                        }
                    }
                }

            }
            return nodes;
        }

        private void Client_KeepAlive(Session sender, KeepAliveEventArgs e)
        {
            if (e.Status != null && ServiceResult.IsNotGood(e.Status))
            {
                Console.WriteLine("{0} {1}/{2}", e.Status, sender.OutstandingRequestCount, sender.DefunctRequestCount);
                if (reconnectHandler == null)
                {
                    Console.WriteLine("--- RECONNECTING ---");
                    reconnectHandler = new SessionReconnectHandler();
                    reconnectHandler.BeginReconnect(sender, ReconnectPeriod * 1000, Client_ReconnectComplete);
                }
            }
        }

        private void Client_ReconnectComplete(object sender, EventArgs e)
        {
            // ignore callbacks from discarded objects.
            if (!Object.ReferenceEquals(sender, reconnectHandler))
            {
                return;
            }
            session = reconnectHandler.Session;
            reconnectHandler.Dispose();
            reconnectHandler = null;
            Trace.WriteLine("Client Reconnected");
        }

        private void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                PlcDataReceivedEventArgs args = new()
                {
                    Name = item.DisplayName,
                    Value = DataValueToNetType(value)
                };
                OnOpcDataReceived(args);
                observers.TryGetValue(item.Subscription.Id, out IObserver observer);
                observer?.Update(args);
            }
        }

        void OnOpcDataReceived(PlcDataReceivedEventArgs args)
        {
            OpcDataReceived?.Invoke(this, args);
        }

        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
            }
        }

        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    session?.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private static object DataValueToNetType(DataValue input)
        {
            if (input?.WrappedValue.TypeInfo.ValueRank == -1)
            {
                return GetSingleObjectFromDataValue(input.Value, input.WrappedValue.TypeInfo.BuiltInType);
            }
            else
            {
                List<object> converedList = new();
                foreach (var item in input.Value as IEnumerable)
                {
                    var converted = GetSingleObjectFromDataValue(item, input.WrappedValue.TypeInfo.BuiltInType);
                    converedList.Add(converted);
                }
                return converedList;
            }
        }
        static object GetSingleObjectFromDataValue(object input, BuiltInType opcType)
        {
            object converted;
            switch (opcType)
            {
                case BuiltInType.Boolean:
                    {
                        converted = Convert.ToBoolean(input);
                        break;
                    }

                case BuiltInType.SByte:
                    {
                        converted = Convert.ToSByte(input);
                        break;
                    }

                case BuiltInType.Byte:
                    {
                        converted = Convert.ToByte(input);
                        break;
                    }

                case BuiltInType.Int16:
                    {
                        converted = Convert.ToInt16(input);
                        break;
                    }

                case BuiltInType.UInt16:
                    {
                        converted = Convert.ToUInt16(input);
                        break;
                    }

                case BuiltInType.Int32:
                    {
                        converted = Convert.ToInt32(input);
                        break;
                    }

                case BuiltInType.UInt32:
                    {
                        converted = Convert.ToUInt32(input);
                        break;
                    }

                case BuiltInType.Int64:
                    {
                        converted = Convert.ToInt64(input);
                        break;
                    }

                case BuiltInType.UInt64:
                    {
                        converted = Convert.ToUInt64(input);
                        break;
                    }

                case BuiltInType.Float:
                    {
                        converted = Convert.ToSingle(input);
                        break;
                    }

                case BuiltInType.Double:
                    {
                        converted = Convert.ToDouble(input);
                        break;
                    }

                default:
                    {
                        converted = input;
                        break;
                    }
            }
            return converted;
        }

    }
}
