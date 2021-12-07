
using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Data.RecipeAggregate;
using KaRecipes.BL.Data.RequestAggregate;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    public interface IRequest
    {
        RequestData Acknowedgle { get; set; }
        RequestData Command { get; set; }
        PartData Data { get; set; }
        RequestData PartId { get; set; }
        RequestData Error { get; set; }

        Task ExecuteStart(PartData stationData);
        Task ExecuteStop();
    }
}