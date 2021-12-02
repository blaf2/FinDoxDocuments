using System.ComponentModel.DataAnnotations;

namespace FinDoxDocumentsAPI.Models
{
    public class CreateUserRequest : ICanConvertToDbModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [Password]
        public string Password { get; set; }

        [Required]
        public UserTypes UserType { get; set; }

        public object GetDbModel()
        {
            return new { user_name = UserName, password = Password, user_type = UserType };
        }
    }
}
