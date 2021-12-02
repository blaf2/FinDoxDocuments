using System.ComponentModel.DataAnnotations;

namespace FinDoxDocumentsAPI.Models
{
    public class UpdateUserRequest : ICanConvertToDbModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Password]
        public string Password { get; set; }

        [Required]
        public UserTypes UserType { get; set; }

        public object GetDbModel()
        {
            return new { id = Id, user_name = UserName, password = Password, user_type = UserType };
        }
    }
}
