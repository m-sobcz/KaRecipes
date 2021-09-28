using KaRecipes.DA.OPC;
using System;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    class Program
    {
        static Client client = new();
        static void Main(string[] args)
        {

            Task t=Task.Run(()=>Multitasker());
            t.Wait();
            Console.WriteLine("Multitasker done");          
        }
        static async Task Multitasker() 
        {
            Console.CancelKeyPress += (sender, args) => client.Close();
            client.NodeIdentifiers.Add("Siemens.M01.OPC_UA_T.zmiena1");
            client.NodeIdentifiers.Add("Siemens.M01.OPC_UA_T.zmiena2");
            Console.WriteLine("Client starting....");
            await client.Start();    
            Console.WriteLine("Client started! Waiting...");
            await Task.Delay(3000);
            Console.WriteLine("Client closing...");
            client.Close();
            Console.WriteLine("Client closed! Waiting... ");
            await Task.Delay(300);
        }


    }
}
