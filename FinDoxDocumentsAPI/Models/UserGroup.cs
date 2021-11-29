using System.Collections.Generic;

namespace FinDoxDocumentsAPI.Models
{
    public class UserGroup
    {
        public int UserGroupId { get; set; }
        public string UserGroupName { get; set; }
        public IList<User> Members { get; set; }
    }
}
