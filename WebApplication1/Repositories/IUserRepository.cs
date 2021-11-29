using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(int id);
        Task<User> CreateUserAsync(CreateUserRequest user);
        Task<User> UpdateUserAsync(int id, UpdateUserRequest user);
        Task DeleteUserAsync(int id);
        Task<User> GetUserFromCredientialsAsync(UserInfo userInfo);
    }
}
