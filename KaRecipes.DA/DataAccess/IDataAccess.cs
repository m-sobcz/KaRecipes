using System;
using System.Collections.Generic;

namespace KaRecipes.DA.DataAccess
{
    public interface IDataAccess
    {
        int Delete(string storedProcedureName, object parameter);
        List<T> Load<T>(string storedProcedureName, object parameter = null);
        List<Tout> Load<Tin1, Tin2, Tout>(string storedProcedureName, Func<Tin1, Tin2, Tout> mapping, object parameter, string splitOn);
        int? Save<T>(string StoredProcedureName, T data);
    }
}