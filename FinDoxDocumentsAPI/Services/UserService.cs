using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await _userRepository.GetUsersAsync();
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _userRepository.GetUserAsync(id);
        }

        public async Task<User> GetUserFromCredientialsAsync(UserInfo userInfo)
        {
            return await _userRepository.GetUserFromCredientialsAsync(userInfo);
        }

        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            return await _userRepository.CreateUserAsync(request);
        }

        public async Task<User> UpdateUserAsync(UpdateUserRequest request)
        {
            return await _userRepository.UpdateUserAsync(request);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteUserAsync(id);
        }
    }
}
