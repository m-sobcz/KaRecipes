using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace KaRecipes.DA.OPC
{
    public class Client
    {
        ApplicationInstance opcApplication;
        const int ReconnectPeriod = 10;
        Session session;
        SessionReconnectHandler reconnectHandler;
        string endpointURL = "opc.tcp://DESKTOP-EQ7QSF9:49320";
        int clientRunTime = Timeout.Infinite;

        public Client()
        {
            opcApplication = new ApplicationInstance
            {
                ApplicationName = "KaRecipes OPC UA Client",
                ApplicationType = ApplicationType.Client,
                ConfigSectionName ="OPC"
            };
        }
        public async Task<int> Run() 
        {
            ApplicationConfiguration config = await opcApplication.LoadApplicationConfiguration(false);
            bool haveAppCertificate = await opcApplication.CheckApplicationInstanceCertificate(false, 0);
            config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
            config.ApplicationUri = X509Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);
            var selectedEndpoint = CoreClientUtils.SelectEndpoint(endpointURL, haveAppCertificate, 15000);
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(null, selectedEndpoint, endpointConfiguration);
            session = await Session.Create(config, endpoint, false, "OPC UA Console Client", 60000, new UserIdentity(new AnonymousIdentityToken()), null);
            session.KeepAlive += Client_KeepAlive;
            Console.WriteLine(" DisplayName, BrowseName, NodeClass");
            NodeId nodeId1 = new NodeId("Siemens.M01.OPC_UA_T.zmiena1", 2);
            var uris = session.NamespaceUris;
            var val1 = session.ReadValue(nodeId1);
            Console.WriteLine(val1.WrappedValue.TypeInfo + " " + val1.WrappedValue.Value);
            NodeId nodeId2 = new NodeId("Siemens.M01.OPC_UA_T.zmiena2", 2);
            var val2 = session.ReadValue(nodeId2);
            Console.WriteLine(val2.WrappedValue.TypeInfo + " " + val2.WrappedValue.Value);


            Console.WriteLine("5 - Create a subscription with publishing interval of 1 second.");
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };
            var list = new List<MonitoredItem> {
                new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i="+Variables.Server_ServerStatus_CurrentTime.ToString()
                }
            };
            list.ForEach(i => i.Notification += OnNotification);
            subscription.AddItems(list);
            session.AddSubscription(subscription);
            subscription.Create();
            return 1;
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

        private static void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
            }
        }

        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
            {
                e.Accept = true;
            }
        }


    }
}
