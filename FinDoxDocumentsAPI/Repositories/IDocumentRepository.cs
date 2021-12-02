using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IDocumentRepository
    {
        Task<DocumentMetadata> GetDocumentAsync(int id, User user);
        Task<IEnumerable<DocumentMetadata>> GetUserDocumentsAsync(int userId);
        Task<DocumentContent> DownloadDocumentAsync(int id, User user);
        Task<DocumentMetadata> UpdateDocumentAsync(UpdateDocumentRequest request, User user);
        Task<DocumentMetadata> UploadDocumentAsync(UploadDocumentRequest request);
        Task DeleteDocumentAsync(int id, User user);
        Task<IEnumerable<DocumentMetadata>> SearchDocumentsAsync(DocumentSearchCriteria criteria, User user);
    }
}
