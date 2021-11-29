namespace FinDoxDocumentsAPI.Models
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserTypes UserType { get; set; }
    }
}
