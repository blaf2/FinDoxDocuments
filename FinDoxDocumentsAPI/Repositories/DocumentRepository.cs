using Dapper;
using FinDoxDocumentsAPI.Models;
using Npgsql;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public DocumentRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task DeleteDocumentAsync(int id, User user)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("user_id", user.UserId);
                    parameters.Add("check_user", user.UserType != UserTypes.Admin);
                    await connection.QueryAsync("delete_document", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<Document> DownloadDocumentAsync(int id, User user)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("user_id", user.UserId);
                    parameters.Add("check_user", user.UserType != UserTypes.Admin);
                    var result = await connection.QueryAsync<Document>("get_document", parameters, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<Document> UpdateDocumentAsync(int id, UpdateDocumentRequest request, User user)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("_document_name", request.DocumentName);
                    parameters.Add("_description", request.Description);
                    parameters.Add("_category", request.Category);
                    parameters.Add("users", request.Users?.Select(x => x.UserId).ToArray());
                    parameters.Add("user_groups", request.Groups?.Select(x => x.UserGroupId).ToArray());
                    parameters.Add("user_id", user.UserId);
                    parameters.Add("check_user", user.UserType != UserTypes.Admin);
                    var result = await connection.QueryAsync<Document>("update_document", parameters, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }    
        }

        public async Task<Document> UploadDocumentAsync(UploadDocumentRequest request)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_document_name", request.DocumentName);
                    parameters.Add("_description", request.Description);
                    parameters.Add("_category", request.Category);
                    parameters.Add("_document_content", request.DocumentContent);
                    parameters.Add("users", request.Users?.Select(x => x.UserId).ToArray());
                    parameters.Add("user_groups", request.Groups?.Select(x => x.UserGroupId).ToArray());
                    var result = await connection.QueryAsync<Document>("new_document", parameters, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
    }
}
