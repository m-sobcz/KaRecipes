using KaRecipes.DA.OPC;
using System;

namespace KaRecipes.IntegrationTests
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new();
            var x = client.Run()
            Console.ReadKey();
        }
    }
}
