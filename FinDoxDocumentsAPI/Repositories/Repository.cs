using Dapper;
using FinDoxDocumentsAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public class Repository : IRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public Repository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<T> DatabaseCallAsync<T>(Func<IDbConnection, DynamicParameters, Task<T>> databaseCall, ICanConvertToDbModel request = null, Dictionary<string, object> input = null)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = request == null ? new DynamicParameters() : new DynamicParameters(request.GetDbModel());
                    if (input != null)
                    {
                        parameters.AddDynamicParams(input);
                    }
                    return await databaseCall(connection, parameters);
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }

        public async Task<T> DocumentDatabaseCallAsync<T>(Func<IDbConnection, DynamicParameters, Task<T>> databaseCall, ICanConvertToDbModel request = null, Dictionary<string, object> input = null)
        {
            using (var connection = _dbConnectionFactory.GetDocumentConnection())
            {
                try
                {
                    var parameters = request == null ? new DynamicParameters() : new DynamicParameters(request.GetDbModel());
                    if (input != null)
                    {
                        parameters.AddDynamicParams(input);
                    }
                    return await databaseCall(connection, parameters);
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message, ex);
                }
            }
        }
    }
}
