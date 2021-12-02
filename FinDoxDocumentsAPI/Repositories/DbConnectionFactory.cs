using Npgsql;
using System.Data;

namespace FinDoxDocumentsAPI.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private readonly string _documentConnectionString;

        public DbConnectionFactory(string connectionString, string documentConnectionString)
        {
            _connectionString = connectionString;
            _documentConnectionString = documentConnectionString;
        }

        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public IDbConnection GetDocumentConnection()
        {
            return new NpgsqlConnection(_documentConnectionString);
        }
    }
}
