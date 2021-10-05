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
            var readVal = client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena1").Result;
            Console.WriteLine(readVal.Value);
            readVal = client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena2").Result;
            Console.WriteLine(readVal.Value);
            Console.WriteLine("Create node dictionaries...");
            client.GetAvailableNodeTypes().Wait();
            Console.WriteLine("Create subscriptions...");
            List<string> subscriptions = new()
            {
                "KaRecipes.M01.OPC_UA_T.zmiena1",
                "KaRecipes.M01.OPC_UA_T.zmiena2"
            };
            client.CreateSubscriptionsWithInterval(subscriptions, 1000).Wait();
            client.OpcDataReceived += Client_opcDataReceived;
            bool res = client.WriteParameter("KaRecipes.M01.OPC_UA_T.zmiena1", (short)2).Result;

            Console.WriteLine("Press any key to end...");
            Console.ReadKey();
            Console.WriteLine("Closing client... ");

            //Task t=Task.Run(()=>Multitasker());
            //t.Wait();
            //Console.WriteLine("Multitasker done");          
        }
        static async Task Multitasker() 
        {
            using OpcClient client = new();
            //Console.CancelKeyPress += (sender, args) => client.Close();
            Console.WriteLine("Create client...");
            await client.Start();
            Console.WriteLine("Read val...");
            var readVal = await client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena1");
            Console.WriteLine(readVal.Value);
            readVal = await client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena2");
            Console.WriteLine(readVal.Value);
            Console.WriteLine("Create node dictionaries...");
            await client.GetAvailableNodeTypes();
            Console.WriteLine("Create subscriptions...");
            List<string> subscriptions = new()
            {
                "KaRecipes.M01.OPC_UA_T.zmiena1",
                "KaRecipes.M01.OPC_UA_T.zmiena2"
            };
            await client.CreateSubscriptionsWithInterval(subscriptions,1000);
            client.OpcDataReceived += Client_opcDataReceived;
            bool res=await client.WriteParameter("KaRecipes.M01.OPC_UA_T.zmiena1", (short)2);
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
