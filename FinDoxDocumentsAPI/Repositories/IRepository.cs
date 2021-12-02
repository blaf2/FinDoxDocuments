using Dapper;
using FinDoxDocumentsAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IRepository
    {
        Task<T> DatabaseCallAsync<T>(Func<IDbConnection, DynamicParameters, Task<T>> databaseCall, ICanConvertToDbModel request = null, Dictionary<string, object> input = null);
    }
}
