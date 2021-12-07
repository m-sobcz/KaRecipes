using KaRecipes.BL.Data.PartAggregate;
using KaRecipes.BL.Data.RequestAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Part
{
    public interface IRequestCommand
    {
        Task<PartData> Execute(PartData Data);
    }
}
