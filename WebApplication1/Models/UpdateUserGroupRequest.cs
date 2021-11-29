using System.Collections.Generic;

namespace FinDoxDocumentsAPI.Models
{
    public class UpdateUserGroupRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<User> Members { get; set; }
    }
}
