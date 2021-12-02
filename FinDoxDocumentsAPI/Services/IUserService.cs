using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> GetUserFromCredientialsAsync(UserInfo userInfo);
        Task<User> CreateUserAsync(CreateUserRequest request);
        Task<User> UpdateUserAsync(UpdateUserRequest request);
        Task DeleteUserAsync(int id);
    }
}
