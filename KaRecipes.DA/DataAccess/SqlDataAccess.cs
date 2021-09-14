using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Data.SqlClient;

namespace KaRecipes.DA.DataAccess
{
    public class SqlDataAccess
    {
        readonly string connectionString;
        public SqlDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<T> Load<T>(string storedProcedureName, object parameter = null)
        {
            using var connection = GetDbConnection();
            return connection.Query<T>(storedProcedureName, parameter, commandType: CommandType.StoredProcedure).ToList();
        }
        public List<Tout> Load<Tin1, Tin2, Tout>(string storedProcedureName, Func<Tin1, Tin2, Tout> mapping, object parameter, string splitOn)
        {
            using IDbConnection connection = GetDbConnection();
            return connection.Query<Tin1, Tin2, Tout>(storedProcedureName, mapping, parameter, splitOn: splitOn).ToList();
        }
        public int? Save<T>(string StoredProcedureName, T data)
        {
            using var connection = GetDbConnection();
            object result = connection.ExecuteScalar(StoredProcedureName, data, commandType: CommandType.StoredProcedure);
            return (int.TryParse(result?.ToString(), out int intResult)) ? intResult : null;
        }
        public int Delete(string storedProcedureName, object parameter)
        {
            using var connection = GetDbConnection();
            return connection.Execute(storedProcedureName, parameter, commandType: CommandType.StoredProcedure);          
        }
        IDbConnection GetDbConnection() 
        {
            return new SqlConnection(connectionString);
        }
    }
}
