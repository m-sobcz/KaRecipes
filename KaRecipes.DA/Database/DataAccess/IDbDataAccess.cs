using System.Collections.Generic;
using System.Data;

namespace KaRecipes.DA.Database.DataAccess
{
    public interface IDbDataAccess
    {
        int Delete(string sql, object parameter, CommandType commandType = CommandType.StoredProcedure);
        List<T> Load<T>(string sql, object parameter = null, CommandType commandType = CommandType.StoredProcedure);
        int? Save<T>(string sql, T data, CommandType commandType = CommandType.StoredProcedure);
    }
}