using System.Collections.Generic;

namespace FinDoxDocumentsAPI.Models
{
    public class UploadDocumentRequest
    {
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public DocumentCategories Category { get; set; }
        public byte[] DocumentContent { get; set; }
        public IList<User> Users { get; set; }
        public IList<UserGroup> Groups { get; set; }
    }
}
