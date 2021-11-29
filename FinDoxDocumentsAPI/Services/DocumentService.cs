using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Repositories;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task DeleteDocumentAsync(int id, User user)
        {
            await _documentRepository.DeleteDocumentAsync(id, user);
        }

        public async Task<Document> DownloadDocumentAsync(int id, User user)
        {
            return await _documentRepository.DownloadDocumentAsync(id, user);
        }

        public async Task<Document> UpdateDocumentAsync(int id, UpdateDocumentRequest request, User user)
        {
            return await _documentRepository.UpdateDocumentAsync(id, request, user);
        }

        public async Task<Document> UploadDocumenAsync(UploadDocumentRequest request)
        {
            return await _documentRepository.UploadDocumentAsync(request);
        }
    }
}
