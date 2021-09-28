using KaRecipes.DA.OPC;
using System;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            multitasker().Wait();
        }
        static async Task multitasker() 
        {
            while (true) 
            { 
                Client client = new();
                await client.Run();
            }
        }

    }
}
