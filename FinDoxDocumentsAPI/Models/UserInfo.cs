using System.ComponentModel.DataAnnotations;

namespace FinDoxDocumentsAPI.Models
{
    public class UserInfo : ICanConvertToDbModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public object GetDbModel()
        {
            return new { user_name = UserName, password = Password };
        }
    }
}
