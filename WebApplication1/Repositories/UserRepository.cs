using FinDoxDocumentsAPI.Models;
using Npgsql;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using System.Linq;
using System;

namespace FinDoxDocumentsAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public UserRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using(var connection = _dbConnectionFactory.GetConnection())
            {
                return await connection.QueryAsync<User>("get_user", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<User> GetUserAsync(int id)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("id", id);
                var result = await connection.QueryAsync<User>("get_user", parameters, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
        }

        public async Task<User> CreateUserAsync(CreateUserRequest user)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("user_name", user.UserName);
                    parameters.Add("password", user.Password);
                    parameters.Add("user_type", user.UserType);
                    var result = await connection.QueryAsync<User>("new_user", parameters, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<User> UpdateUserAsync(int id, UpdateUserRequest user)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("user_name", user.UserName);
                    parameters.Add("password", user.Password);
                    parameters.Add("user_type", user.UserType);
                    var result = await connection.QueryAsync<User>("update_user", parameters, commandType: CommandType.StoredProcedure);
                    return result.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    await connection.QueryAsync("delete_user", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<User> GetUserFromCredientialsAsync(UserInfo userInfo)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add("user_name", userInfo.UserName);
                parameters.Add("password", userInfo.Password);
                var result = await connection.QueryAsync<User>("get_user_from_credentials", parameters, commandType: CommandType.StoredProcedure);
                return result?.FirstOrDefault();
            }
        }
    }
}
