using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaRecipes.BL.Interfaces
{
    public interface IDbDataAccess<T> : IAggregateRoot
    {
        Task<T> Read();
        Task<int?> Write(T data);
    }
}
