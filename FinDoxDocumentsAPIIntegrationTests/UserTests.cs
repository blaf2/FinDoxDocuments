using FinDoxDocumentsAPI.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace FixDoxDocumentsAPIIntegrationTests
{
    public class UserTests : IntegrationTest
    {
        public UserTests(ApiWebApplicationFactory fixture) : base(fixture) { }

        [Fact]
        public async Task GetCreateUpdateDeleteUserTest()
        {
            var user = new User { UserId = _adminUserId, UserName = "admin", UserType = UserTypes.Admin };
            var newUser = new User { UserId = _adminUserId + 1, UserName = "user2", UserType = UserTypes.Regular };
            var newUserRequest = new CreateUserRequest { UserName = newUser.UserName, Password = "password2", UserType = newUser.UserType };
            var updatedUser = new User { UserId = _adminUserId + 1, UserName = "updatedUser2", UserType = UserTypes.Manager };
            var updateUserRequest = new UpdateUserRequest { Id = updatedUser.UserId, UserName = updatedUser.UserName, Password = "updatedPassword", UserType = updatedUser.UserType };
            var adminUserInfo = new UserInfo { UserName = user.UserName, Password = "password" };

            var tokenResponse = await _client.PostAsJsonAsync("/api/token", adminUserInfo);
            tokenResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var token = await tokenResponse.Content.ReadAsStringAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(newUser);

            var updateResponse = await _client.PutAsJsonAsync($"/api/users/{newUser.UserId}", updateUserRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedUserResult = await updateResponse.Content.ReadAsJsonAsync<User>();
            updatedUserResult.Should().BeEquivalentTo(updatedUser);

            var deleteResponse = await _client.DeleteAsync($"/api/users/{newUser.UserId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await _client.GetAsync($"/api/users/{newUser.UserId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getAllResponse = await _client.GetAsync("/api/users");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await getAllResponse.Content.ReadAsJsonAsync<List<User>>();
            users.Count.Should().Be(1);
            users[0].Should().BeEquivalentTo(user);
        }
    }
}
