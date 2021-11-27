using KaRecipes.BL;
using KaRecipes.BL.Data;
using KaRecipes.BL.Interfaces;
using KaRecipes.DA.OPC;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    class Program
    {
        static OpcClient client = new();
        static void Main(string[] args)
        {
            using OpcClient client = new();
            //Console.CancelKeyPress += (sender, args) => client.Close();
            Console.WriteLine("Create client...");
            client.Start().Wait();
            Console.WriteLine("Read val...");
            var readVal = client.ReadDataNode("KaRecipes.M01.OPC_UA_T.zmiena1").Result;
            Console.WriteLine(readVal.Value);
            readVal = client.ReadDataNode("KaRecipes.M01.OPC_UA_T.zmiena2").Result;
            Console.WriteLine(readVal.Value);
            Console.WriteLine("Create node dictionaries...");
            //var nodes = client.GetAvailableNodes().Result;
            Console.WriteLine("Create subscriptions...");
            List<string> subscriptions = new()
            {
                "KaRecipes.M01.OPC_UA_T.zmiena1",
                "KaRecipes.M01.OPC_UA_T.zmiena2",
                "KaRecipes.M01.OPC_UA_T.tab"
            };
            var observer = new Observer();
            client.CreateSubscriptionsWithInterval(subscriptions, 1000,observer).Wait();
            //client.OpcDataReceived += Client_opcDataReceived;
            List<DataNode> dataNodesToWrite = new() 
            { 
                new DataNode() { NodeId = "KaRecipes.M01.OPC_UA_T.zmiena1", Value = (short)1000},
                new DataNode() { NodeId = "KaRecipes.M01.OPC_UA_T.zmiena2", Value = false },
            };
            bool res = client.WriteDataNodes(dataNodesToWrite).Result;

            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
            Console.WriteLine("Closing client... ");      
        }


        private static void Client_opcDataReceived(object sender, PlcDataReceivedEventArgs e)
        {
            Console.WriteLine(e.Name + ":   " + e.Value +"<" +e.Value.GetType()+">");
        }
    }
}
