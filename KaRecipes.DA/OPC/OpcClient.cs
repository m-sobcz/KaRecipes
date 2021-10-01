using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KaRecipes.BL.Interfaces;
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
        public event EventHandler<PlcDataReceivedEventArgs> OpcDataReceived;
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
        public object ReadNode(string nodeIdentifier)
        {
            NodeId nodeId1 = new(nodeIdentifier, 2);
            var readVal = session.ReadValue(nodeId1);
            var convertedVal = DataValueToNetType(readVal);
            return convertedVal;
        }

        public void CreateSubscriptions(List<string> monitoredNodeIdentifiers)
        {
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };
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
            session.AddSubscription(subscription);
            subscription.Create();
        }

        public void WriteToNode(string nodeIdentifier, object value)
        {
            WriteValue valueToWrite = new WriteValue();

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

            session.Write(
                null,
                valuesToWrite,
                out results,
                out diagnosticInfos);

            ClientBase.ValidateResponse(results, valuesToWrite);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, valuesToWrite);

            if (StatusCode.IsBad(results[0]))
            {
                throw new ServiceResultException(results[0]);
            }
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
        private object DataValueToNetType(DataValue input)
        {
            object converted;
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
