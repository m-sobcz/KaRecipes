using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace KaRecipes.BL.PartData
{
    public interface IRequest
    {
        RequestData Acknowedgle { get; set; }
        RequestData Command { get; set; }
        ConcurrentDictionary<string, RequestData> Data { get; set; }
        RequestData Error { get; set; }

        Task Start();
        Task Stop();
    }
}