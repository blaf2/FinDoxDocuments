using Dapper;
using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public class DocumentRepository : Repository, IDocumentRepository
    {
        public DocumentRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task DeleteDocumentAsync(int id, User user)
        {
            await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync("documents.delete_document", parameters, commandType: CommandType.StoredProcedure);
            }, input: new Dictionary<string, object> { { "id", id }, { "user_id", user.UserId }, { "check_user", user.UserType != UserTypes.Admin } });

            await DocumentDatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync("documents.delete_document", parameters, commandType: CommandType.StoredProcedure);
            }, input: new Dictionary<string, object> { { "metadata_id", id } });
        }

        public async Task<DocumentContent> DownloadDocumentAsync(int id, User user)
        {
            var metadata = await GetDocumentAsync(id, user);

            return await DocumentDatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<DocumentContent>("documents.get_document", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, input: new Dictionary<string, object> { { "metadata_id", metadata.DocumentId } });
        }

        public async Task<DocumentMetadata> GetDocumentAsync(int id, User user)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<DocumentMetadata>("documents.get_document", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, input: new Dictionary<string, object> { { "id", id }, { "user_id", user.UserId }, { "check_user", user.UserType != UserTypes.Admin } });
        }

        public async Task<IEnumerable<DocumentMetadata>> GetUserDocumentsAsync(int userId)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync<DocumentMetadata>("documents.get_documents_by_user", parameters, commandType: CommandType.StoredProcedure);
            }, input: new Dictionary<string, object> { { "user_id", userId }});
        }

        public async Task<IEnumerable<DocumentMetadata>> SearchDocumentsAsync(DocumentSearchCriteria criteria, User user)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync<DocumentMetadata>("documents.search_documents", parameters, commandType: CommandType.StoredProcedure);
            }, criteria, new Dictionary<string, object> { { "user_id", user.UserId }, { "check_user", user.UserType != UserTypes.Admin } });
        }

        public async Task<DocumentMetadata> UpdateDocumentAsync(UpdateDocumentRequest request, User user)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<DocumentMetadata>("documents.update_document", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, request, new Dictionary<string, object> { { "user_id", user.UserId }, { "check_user", user.UserType != UserTypes.Admin } });
        }

        public async Task<DocumentMetadata> UploadDocumentAsync(UploadDocumentRequest request)
        {
            var documentMetadata = await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<DocumentMetadata>("documents.new_document", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, request);

            var uploadRequest = new DocumentContent { MetadataId = documentMetadata.DocumentId, Content = request.DocumentContent };

            await DocumentDatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<DocumentContent>("documents.new_document", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, uploadRequest);

            return documentMetadata;
        }
    }
}
