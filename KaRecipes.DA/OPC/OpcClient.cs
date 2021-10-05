using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        Session session;
        SessionReconnectHandler reconnectHandler;
        readonly string nodeIdPrefix = "KaRecipes";
        public event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;

        readonly Regex nodeNameRegex = new(@"(?<=\.)\w+\b(?!\.)", RegexOptions.Compiled);

        public string PlcAccessPrefix => nodeIdPrefix;

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
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointUrl, haveAppCertificate, 15000);
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            session = await Session.Create(config, endpoint, false, opcApplication.ApplicationName, 60000, new UserIdentity(new AnonymousIdentityToken()), null);
            session.KeepAlive += Client_KeepAlive;
        }

        public async Task<ParameterSingle> ReadParameter(string nodeIdentifier)
        {
            var readVal = await ReadNode(nodeIdentifier);
            var convertedVal = DataValueToNetType(readVal);
            var name = ExtractNameFromIdentifier(nodeIdentifier);
            return new ParameterSingle() { Name = name, Value = convertedVal }; ;
        }

        string ExtractNameFromIdentifier(string nodeIdentifier)
        {
            var match = nodeNameRegex.Match(nodeIdentifier);
            var name = match.Value;
            return name;
        }

        async Task<DataValue> ReadNode(string nodeIdentifier)
        {
            NodeId nodeId1 = new(nodeIdentifier, 2);
            var readVal = await Task.Run(() => session.ReadValue(nodeId1));
            return readVal;
        }

        public async Task CreateSubscriptionsWithInterval(List<string> monitoredNodeIdentifiers, int publishingInterval)
        {
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = publishingInterval };
            var MonitoredItems = new List<MonitoredItem>
            {
                new MonitoredItem(subscription.DefaultItem) { DisplayName = "ServerStatusCurrentTime", StartNodeId = "i=" + Variables.Server_ServerStatus_CurrentTime.ToString() },
            };
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
        }

        public async Task<bool> WriteParameter(string nodeIdentifier, object value)
        {
            WriteValue valueToWrite = new();
            valueToWrite.NodeId = new NodeId(nodeIdentifier, namespaceIndex);
            valueToWrite.AttributeId = Attributes.Value;
            valueToWrite.Value.Value = value;
            valueToWrite.Value.StatusCode = StatusCodes.Good;
            valueToWrite.Value.ServerTimestamp = DateTime.MinValue;
            valueToWrite.Value.SourceTimestamp = DateTime.MinValue;

            WriteValueCollection valuesToWrite = new WriteValueCollection();
            valuesToWrite.Add(valueToWrite);

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
            Console.WriteLine("--- RECONNECTED ---");
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
            object converted;
            if (input?.WrappedValue.TypeInfo.ValueRank != -1) return null;//ignore arrays
            switch (input?.WrappedValue.TypeInfo.BuiltInType)
            {
                case BuiltInType.Boolean:
                    {
                        converted = Convert.ToBoolean(input.Value);
                        break;
                    }

                case BuiltInType.SByte:
                    {
                        converted = Convert.ToSByte(input.Value);
                        break;
                    }

                case BuiltInType.Byte:
                    {
                        converted = Convert.ToByte(input.Value);
                        break;
                    }

                case BuiltInType.Int16:
                    {
                        converted = Convert.ToInt16(input.Value);
                        break;
                    }

                case BuiltInType.UInt16:
                    {
                        converted = Convert.ToUInt16(input.Value);
                        break;
                    }

                case BuiltInType.Int32:
                    {
                        converted = Convert.ToInt32(input.Value);
                        break;
                    }

                case BuiltInType.UInt32:
                    {
                        converted = Convert.ToUInt32(input.Value);
                        break;
                    }

                case BuiltInType.Int64:
                    {
                        converted = Convert.ToInt64(input.Value);
                        break;
                    }

                case BuiltInType.UInt64:
                    {
                        converted = Convert.ToUInt64(input.Value);
                        break;
                    }

                case BuiltInType.Float:
                    {
                        converted = Convert.ToSingle(input.Value);
                        break;
                    }

                case BuiltInType.Double:
                    {
                        converted = Convert.ToDouble(input.Value);
                        break;
                    }

                default:
                    {
                        converted = input.Value;
                        break;
                    }
            }

            return converted;
        }
    }
}
