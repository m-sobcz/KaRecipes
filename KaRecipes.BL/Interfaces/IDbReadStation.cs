using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    interface IDbReadStation<T>
    {
        Task<T> Read(T objectToFill);
    }
}
