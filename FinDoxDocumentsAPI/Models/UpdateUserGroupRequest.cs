using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FinDoxDocumentsAPI.Models
{
    public class UpdateUserGroupRequest : ICanConvertToDbModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public IList<User> Members { get; set; }

        public object GetDbModel()
        {
            return new { id = Id, _user_group_name = Name, members = Members.Select(x => x.UserId).ToArray() };
        }
    }
}
