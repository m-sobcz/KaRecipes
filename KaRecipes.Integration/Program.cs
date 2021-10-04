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
            var readVal = await client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena1");
            Console.WriteLine(readVal.Value);
            readVal = await client.ReadParameter("KaRecipes.M01.OPC_UA_T.zmiena2");
            Console.WriteLine(readVal.Value);
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
