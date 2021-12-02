using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinDoxDocumentsAPI.Models
{
    public class CreateUserGroupRequest : ICanConvertToDbModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IList<User> Members { get; set; }

        public object GetDbModel()
        {
            return new { _user_group_name = Name, members = Members.Select(x => x.UserId).ToArray() };
        }
    }
}
