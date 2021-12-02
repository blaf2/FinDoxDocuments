using Dapper;
using FinDoxDocumentsAPI.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Respawn;
using System.Data;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace FixDoxDocumentsAPIIntegrationTests
{
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>
    {
        protected readonly ApiWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        protected readonly int _adminUserId;

        public IntegrationTest(ApiWebApplicationFactory fixture)
        {
            _factory = fixture;
            _client = _factory.CreateClient();

            using (var connection = new NpgsqlConnection(_factory.Configuration.GetConnectionString("dbConnection")))
            {
                connection.Open();

                var checkpoint = new Checkpoint
                {
                    TablesToIgnore = new string[] { },
                    SchemasToInclude = new string[] { "users", "user_groups", "documents" },
                    DbAdapter = DbAdapter.Postgres
                };

                checkpoint.Reset(connection).Wait();

                var parameters = new DynamicParameters();
                parameters.Add("user_name", "admin");
                parameters.Add("password", "password");
                parameters.Add("user_type", UserTypes.Admin);
                var result = connection.Query<User>("users.new_user", parameters, commandType: CommandType.StoredProcedure);
                _adminUserId = result.First().UserId;
            }

            using (var connection = new NpgsqlConnection(_factory.Configuration.GetConnectionString("dbDocumentConnection")))
            {
                connection.Open();

                var checkpoint = new Checkpoint
                {
                    TablesToIgnore = new string[] { },
                    SchemasToInclude = new string[] { "documents" },
                    DbAdapter = DbAdapter.Postgres
                };

                checkpoint.Reset(connection).Wait();
            }
        }
    }
}
