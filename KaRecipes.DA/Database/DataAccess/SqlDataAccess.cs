using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Configuration;
using System.Data;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Data.SqlClient;

namespace KaRecipes.DA.Database.DataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        readonly string connectionString;
        readonly string connectionName = "KaRecipesDB";
        public SqlDataAccess(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString(connectionName);
        }
        public List<T> Load<T>(string sql, object parameter = null, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = GetDbConnection();
            return connection.Query<T>(sql, parameter, commandType: commandType).ToList();
        }
        public int? Save<T>(string sql, T data, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = GetDbConnection();
            object result = connection.ExecuteScalar(sql, data, commandType: commandType);
            return (int.TryParse(result?.ToString(), out int intResult)) ? intResult : null;
        }
        public int Delete(string sql, object parameter, CommandType commandType = CommandType.StoredProcedure)
        {
            using var connection = GetDbConnection();
            return connection.Execute(sql, parameter, commandType: commandType);
        }
        IDbConnection GetDbConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
