using FinDoxDocumentsAPI.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;

namespace FinDoxDocumentsAPI.Repositories
{
    public class UserRepository : Repository, IUserRepository
    {
        public UserRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync<User>("users.get_user", commandType: CommandType.StoredProcedure);
            });
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<User>("users.get_user", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, input: new Dictionary<string, object> { { "id", id } });
        }

        public async Task<User> CreateUserAsync(CreateUserRequest user)
        {
            return await DatabaseCallAsync<User>(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<User>("users.new_user", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, user);
        }

        public async Task<User> UpdateUserAsync(UpdateUserRequest user)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<User>("users.update_user", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }, user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync("users.delete_user", parameters, commandType: CommandType.StoredProcedure);
            }, input: new Dictionary<string, object> { { "id", id } });
        }

        public async Task<User> GetUserFromCredientialsAsync(UserInfo userInfo)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var result = await connection.QueryAsync<User>("users.get_user_from_credentials", parameters, commandType: CommandType.StoredProcedure);
                return result?.FirstOrDefault();
            }, userInfo);
        }
    }
}
