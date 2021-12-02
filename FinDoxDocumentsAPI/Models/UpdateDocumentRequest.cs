using System.Collections.Generic;
using System.Linq;

namespace FinDoxDocumentsAPI.Models
{
    public class UpdateDocumentRequest : ICanConvertToDbModel
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public DocumentCategories Category { get; set; }
        public IList<User> Users { get; set; }
        public IList<UserGroup> Groups { get; set; }

        public object GetDbModel()
        {
            return new
            {
                id = DocumentId,
                _document_name = DocumentName,
                _description = Description,
                _category = Category,
                users = Users?.Select(x => x.UserId).ToArray(),
                user_groups = Groups?.Select(x => x.UserGroupId).ToArray()
            };
        }
    }
}
