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

            Task t=Task.Run(()=>Multitasker());
            t.Wait();
            Console.WriteLine("Multitasker done");          
        }
        static async Task Multitasker() 
        {
            using OpcClient client = new();
            //Console.CancelKeyPress += (sender, args) => client.Close();
            Console.WriteLine("Create client...");
            await client.Start();
            Console.WriteLine("Read val...");
            var readVal = client.ReadNode("KaRecipes.M01.OPC_UA_T.zmiena1");
            Console.WriteLine(readVal);
            readVal = client.ReadNode("KaRecipes.M01.OPC_UA_T.zmiena2");
            Console.WriteLine(readVal);
            Console.WriteLine("Create subscriptions...");
            List<string> subscriptions = new()
            {
                "KaRecipes.M01.OPC_UA_T.zmiena1",
                "KaRecipes.M01.OPC_UA_T.zmiena2"
            };
            client.CreateSubscriptions(subscriptions);
            client.OpcDataReceived += Client_opcDataReceived;
            await Task.Delay(1000);
            client.WriteToNode("KaRecipes.M01.OPC_UA_T.zmiena1", (short)2);
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
