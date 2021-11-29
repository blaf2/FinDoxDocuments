using FinDoxDocumentsAPI.Models;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public interface IDocumentService
    {
        Task<Document> DownloadDocumentAsync(int id, User user);
        Task<Document> UploadDocumenAsync(UploadDocumentRequest request);
        Task<Document> UpdateDocumentAsync(int id, UpdateDocumentRequest request, User user);
        Task DeleteDocumentAsync(int id, User user);
    }
}
