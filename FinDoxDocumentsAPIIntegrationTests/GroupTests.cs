using FinDoxDocumentsAPI.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace FixDoxDocumentsAPIIntegrationTests
{
    public class GroupTests : IntegrationTest
    {
        public GroupTests(ApiWebApplicationFactory fixture) : base(fixture) { }

        [Fact]
        public async Task GetCreateUpdateDeleteUserGroupTest()
        {
            var user = new User { UserId = _adminUserId, UserName = "admin", UserType = UserTypes.Admin };
            var user2 = new User { UserId = _adminUserId + 1, UserName = "user2", UserType = UserTypes.Regular };
            var user3 = new User { UserId = _adminUserId + 2, UserName = "user3", UserType = UserTypes.Manager };
            var newUserRequest1 = new CreateUserRequest { UserName = user2.UserName, Password = "aA1$aaaa", UserType = user2.UserType };
            var newUserRequest2 = new CreateUserRequest { UserName = user3.UserName, Password = "aA1$aaaa2", UserType = user3.UserType };
            var newUserGroup = new UserGroup { UserGroupName = "userGroup1", Members = new List<User> { user2, user3 } };
            var newUserGroupRequest = new CreateUserGroupRequest { Name = newUserGroup.UserGroupName, Members = newUserGroup.Members };
            var newUserGroup2 = new UserGroup { UserGroupName = "userGroup2", Members = new List<User> { user3 } };
            var newUserGroupRequest2 = new CreateUserGroupRequest { Name = newUserGroup2.UserGroupName, Members = newUserGroup2.Members };
            var updatedUserGroup = new UserGroup { UserGroupName = "updatedUserGroup1", Members = new List<User> { user, user3 } };
            var updateUserGroupRequest = new UpdateUserGroupRequest { Name = updatedUserGroup.UserGroupName, Members = updatedUserGroup.Members };
            var adminUserInfo = new UserInfo { UserName = user.UserName, Password = "password" };
            var managerUserInfo = new UserInfo { UserName = user3.UserName, Password = "aA1$aaaa2" };

            await LoginAsync(adminUserInfo);

            var createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest1);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user2);

            createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest2);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user3);

            createResponse = await _client.PostAsJsonAsync("/api/groups", newUserGroupRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var userGroupResult = await createResponse.Content.ReadAsJsonAsync<UserGroup>();
            newUserGroup.UserGroupId = userGroupResult.UserGroupId;
            updatedUserGroup.UserGroupId = userGroupResult.UserGroupId;
            updateUserGroupRequest.Id = userGroupResult.UserGroupId;
            userGroupResult.Should().BeEquivalentTo(newUserGroup);

            createResponse = await _client.PostAsJsonAsync("/api/groups", newUserGroupRequest2);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            userGroupResult = await createResponse.Content.ReadAsJsonAsync<UserGroup>();
            newUserGroup2.UserGroupId = userGroupResult.UserGroupId;
            userGroupResult.Should().BeEquivalentTo(newUserGroup2);

            var updateResponse = await _client.PutAsJsonAsync($"/api/groups/{newUserGroup.UserGroupId}", updateUserGroupRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedUserGroupResult = await updateResponse.Content.ReadAsJsonAsync<UserGroup>();
            updatedUserGroupResult.Should().BeEquivalentTo(updatedUserGroup);

            var deleteResponse = await _client.DeleteAsync($"/api/groups/{newUserGroup2.UserGroupId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await _client.GetAsync($"/api/groups/{newUserGroup2.UserGroupId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAllResponse = await _client.GetAsync("/api/groups");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var userGroups = await getAllResponse.Content.ReadAsJsonAsync<List<UserGroup>>();
            userGroups.Count.Should().Be(1);
            userGroups[0].Should().BeEquivalentTo(updatedUserGroup);

            await LoginAsync(managerUserInfo);

            createResponse = await _client.PostAsJsonAsync("/api/groups", newUserGroupRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            updateResponse = await _client.PutAsJsonAsync($"/api/groups/{newUserGroup.UserGroupId}", updateUserGroupRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            deleteResponse = await _client.DeleteAsync($"/api/groups/{newUserGroup2.UserGroupId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            getResponse = await _client.GetAsync($"/api/groups/{newUserGroup2.UserGroupId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            getAllResponse = await _client.GetAsync("/api/groups");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        private async Task LoginAsync(UserInfo userInfo)
        {
            var tokenResponse = await _client.PostAsJsonAsync("/api/token", userInfo);
            tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var token = await tokenResponse.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
