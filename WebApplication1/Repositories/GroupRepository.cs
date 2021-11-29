using Dapper;
using FinDoxDocumentsAPI.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public GroupRepository(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<UserGroup> CreateGroupAsync(CreateUserGroupRequest request)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("_user_group_name", request.Name);
                    parameters.Add("members", request.Members.Select(x => x.UserId).ToArray());
                    var lookup = new Dictionary<int, UserGroup>();
                    var results = await connection.QueryAsync<UserGroup, User, UserGroup>("new_user_group",
                        (group, user) =>
                        {
                            if (!lookup.TryGetValue(group.UserGroupId, out UserGroup foundUserGroup))
                            {
                                foundUserGroup = group;
                                foundUserGroup.Members = new List<User>();
                                lookup.Add(group.UserGroupId, foundUserGroup);
                            }
                            foundUserGroup.Members.Add(user);
                            return foundUserGroup;
                        }
                        , parameters
                        , splitOn: "user_id"
                        , commandType: CommandType.StoredProcedure);
                    return lookup.Values.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task DeleteGroupAsync(int id)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    await connection.QueryAsync("delete_user_group", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<UserGroup> GetGroupAsync(int id)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    var lookup = new Dictionary<int, UserGroup>();
                    var results = await connection.QueryAsync<UserGroup, User, UserGroup>("get_user_group",
                        (group, user) =>
                        {
                            if (!lookup.TryGetValue(group.UserGroupId, out UserGroup foundUserGroup))
                            {
                                foundUserGroup = group;
                                foundUserGroup.Members = new List<User>();
                                lookup.Add(group.UserGroupId, foundUserGroup);
                            }
                            foundUserGroup.Members.Add(user);
                            return foundUserGroup;
                        }
                        , parameters
                        , splitOn: "user_id"
                        , commandType: CommandType.StoredProcedure);
                    return lookup.Values.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<IEnumerable<UserGroup>> GetGroupsAsync()
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var lookup = new Dictionary<int, UserGroup>();
                    var results = await connection.QueryAsync<UserGroup, User, UserGroup>("get_user_group",
                        (group, user) =>
                        {
                            if (!lookup.TryGetValue(group.UserGroupId, out UserGroup foundUserGroup))
                            {
                                foundUserGroup = group;
                                foundUserGroup.Members = new List<User>();
                                lookup.Add(group.UserGroupId, foundUserGroup);
                            }
                            foundUserGroup.Members.Add(user);
                            return foundUserGroup;
                        }
                        , splitOn: "user_id"
                        , commandType: CommandType.StoredProcedure);
                    return lookup.Values;
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }

        public async Task<UserGroup> UpdateGroupAsync(int id, UpdateUserGroupRequest request)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("id", id);
                    parameters.Add("_user_group_name", request.Name);
                    parameters.Add("members", request.Members?.Select(x => x.UserId).ToArray());
                    var lookup = new Dictionary<int, UserGroup>();
                    var results = await connection.QueryAsync<UserGroup, User, UserGroup>("update_user_group",
                        (group, user) =>
                        {
                            if (!lookup.TryGetValue(group.UserGroupId, out UserGroup foundUserGroup))
                            {
                                foundUserGroup = group;
                                foundUserGroup.Members = new List<User>();
                                lookup.Add(group.UserGroupId, foundUserGroup);
                            }
                            foundUserGroup.Members.Add(user);
                            return foundUserGroup;
                        }
                        , parameters
                        , splitOn: "user_id"
                        , commandType: CommandType.StoredProcedure);
                    return lookup.Values.FirstOrDefault();
                }
                catch (NpgsqlException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
        }
    }
}
