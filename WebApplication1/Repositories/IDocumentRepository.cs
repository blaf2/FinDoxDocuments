using FinDoxDocumentsAPI.Models;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IDocumentRepository
    {
        Task<Document> DownloadDocumentAsync(int id, User user);
        Task<Document> UpdateDocumentAsync(int id, UpdateDocumentRequest request, User user);
        Task<Document> UploadDocumentAsync(UploadDocumentRequest request);
        Task DeleteDocumentAsync(int id, User user);
    }
}
