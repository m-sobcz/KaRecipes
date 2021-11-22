using KaRecipes.BL.PlcRequest;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.PlcRequest
{
    public interface IRequest
    {
        RequestData Acknowedgle { get; set; }
        RequestData Command { get; set; }
        Dictionary<string, RequestData> Data { get; set; }
        RequestData TargetId { get; set; }
        RequestData Error { get; set; }

        Task ExecuteStart();
        Task ExecuteStop();
    }
}