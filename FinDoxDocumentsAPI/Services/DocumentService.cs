using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Repositories;
using System.Collections.Generic;
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

        public async Task<DocumentContent> DownloadDocumentAsync(int id, User user)
        {
            return await _documentRepository.DownloadDocumentAsync(id, user);
        }

        public async Task<DocumentMetadata> GetDocumentAsync(int id, User user)
        {
            return await _documentRepository.GetDocumentAsync(id, user);
        }

        public async Task<IEnumerable<DocumentMetadata>> GetUserDocumentsAsync(int userId)
        {
            return await _documentRepository.GetUserDocumentsAsync(userId);
        }

        public async Task<IEnumerable<DocumentMetadata>> SearchDocumentsAsync(DocumentSearchCriteria criteria, User user)
        {
            return await _documentRepository.SearchDocumentsAsync(criteria, user);
        }

        public async Task<DocumentMetadata> UpdateDocumentAsync(UpdateDocumentRequest request, User user)
        {
            return await _documentRepository.UpdateDocumentAsync(request, user);
        }

        public async Task<DocumentMetadata> UploadDocumenAsync(UploadDocumentRequest request)
        {
            return await _documentRepository.UploadDocumentAsync(request);
        }
    }
}
