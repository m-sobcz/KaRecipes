using KaRecipes.DA.OPC;
using System;
using System.Threading.Tasks;

namespace KaRecipes.Integration
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new();
            var x = client.Run();
            Console.ReadKey();
        }

    }
}
