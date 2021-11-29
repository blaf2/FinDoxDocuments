using System.Collections.Generic;

namespace FinDoxDocumentsAPI.Models
{
    public class CreateUserGroupRequest
    {
        public string Name { get; set; }
        public IList<User> Members { get; set; }
    }
}
