namespace FinDoxDocumentsAPI.Models
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserTypes UserType { get; set; }
    }
}
