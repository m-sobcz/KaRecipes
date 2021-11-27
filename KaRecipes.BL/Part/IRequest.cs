 using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    public interface IRequest
    {
        RequestData Acknowedgle { get; set; }
        RequestData Command { get; set; }
        Dictionary<string, RequestData> Data { get; set; }
        RequestData PartId { get; set; }
        RequestData Error { get; set; }

        Task ExecuteStart();
        Task ExecuteStop();
    }
}