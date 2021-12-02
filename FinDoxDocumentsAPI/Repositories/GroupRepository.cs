using Dapper;
using FinDoxDocumentsAPI.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Repositories
{
    public class GroupRepository : Repository, IGroupRepository
    {
        public GroupRepository(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }

        public async Task<UserGroup> CreateGroupAsync(CreateUserGroupRequest request)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var lookup = new Dictionary<int, UserGroup>();
                var results = await connection.QueryAsync<UserGroup, User, UserGroup>("user_groups.new_user_group",
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
            }, request);
        }

        public async Task DeleteGroupAsync(int id)
        {
            await DatabaseCallAsync(async (connection, parameters) =>
            {
                return await connection.QueryAsync("user_groups.delete_user_group", parameters, commandType: CommandType.StoredProcedure);
            }, input: new Dictionary<string, object> { { "id", id } });
        }

        public async Task<UserGroup> GetGroupAsync(int id)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var lookup = new Dictionary<int, UserGroup>();
                var results = await connection.QueryAsync<UserGroup, User, UserGroup>("user_groups.get_user_group",
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
            }, input: new Dictionary<string, object> { { "id", id } });
        }

        public async Task<IEnumerable<UserGroup>> GetGroupsAsync()
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var lookup = new Dictionary<int, UserGroup>();
                var results = await connection.QueryAsync<UserGroup, User, UserGroup>("user_groups.get_user_group",
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
                return lookup.Values;
            });
        }

        public async Task<UserGroup> UpdateGroupAsync(UpdateUserGroupRequest request)
        {
            return await DatabaseCallAsync(async (connection, parameters) =>
            {
                var lookup = new Dictionary<int, UserGroup>();
                var results = await connection.QueryAsync<UserGroup, User, UserGroup>("user_groups.update_user_group",
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
            }, request);
        }
    }
}
