using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinDoxDocumentsAPI.Models
{
    public class UploadDocumentRequest : ICanConvertToDbModel
    {
        [Required]
        public string DocumentName { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DocumentCategories Category { get; set; }

        [Required]
        public byte[] DocumentContent { get; set; }

        public IList<User> Users { get; set; }

        public IList<UserGroup> Groups { get; set; }

        public object GetDbModel()
        {
            return new
            {
                _document_name = DocumentName,
                _description = Description,
                _category = Category,
                users = Users?.Select(x => x.UserId).ToArray(),
                user_groups = Groups?.Select(x => x.UserGroupId).ToArray()
            };
        }
    }
}
