using FinDoxDocumentsAPI.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace FixDoxDocumentsAPIIntegrationTests
{
    public class DocumentTests : IntegrationTest
    {
        public DocumentTests(ApiWebApplicationFactory fixture) : base(fixture) { }

        [Fact]
        public async Task GetCreateUpdateDeleteDocumentTest()
        {
            var user = new User { UserId = _adminUserId, UserName = "admin", UserType = UserTypes.Admin };
            var user2 = new User { UserId = _adminUserId + 1, UserName = "user2", UserType = UserTypes.Regular };
            var user3 = new User { UserId = _adminUserId + 2, UserName = "user3", UserType = UserTypes.Manager };
            var user4 = new User { UserId = _adminUserId + 3, UserName = "user4", UserType = UserTypes.Regular };
            var user5 = new User { UserId = _adminUserId + 4, UserName = "user5", UserType = UserTypes.Manager };
            var newUserRequest1 = new CreateUserRequest { UserName = user2.UserName, Password = "aA1$aaaa2", UserType = user2.UserType };
            var newUserRequest2 = new CreateUserRequest { UserName = user3.UserName, Password = "aA1$aaaa3", UserType = user3.UserType };
            var newUserRequest3 = new CreateUserRequest { UserName = user4.UserName, Password = "aA1$aaaa4", UserType = user4.UserType };
            var newUserRequest4 = new CreateUserRequest { UserName = user5.UserName, Password = "aA1$aaaa5", UserType = user5.UserType };
            var userGroup = new UserGroup { UserGroupName = "userGroup1", Members = new List<User> { user2, user3 } };
            var newUserGroupRequest = new CreateUserGroupRequest { Name = userGroup.UserGroupName, Members = userGroup.Members };
            var document1 = new DocumentMetadata { DocumentName = "docName", Description = "docDesc", Category = DocumentCategories.PNG };
            var documentContent1 = new DocumentContent { Content = new byte[] { 23, 34, 32, 32, 54, 23, 23 } };
            var newDocumentRequest1 = new UploadDocumentRequest
            {
                DocumentName = document1.DocumentName,
                Description = document1.Description,
                Category = document1.Category,
                DocumentContent = documentContent1.Content,
                Users = new List<User> { user4 },
                Groups = new List<UserGroup> { userGroup }
            };
            var document2 = new DocumentMetadata { DocumentName = "docName2", Description = "docDesc2", Category = DocumentCategories.PDF };
            var documentContent2 = new DocumentContent { Content = new byte[] { 23, 3, 32, 3, 54, 23, 23 } };
            var newDocumentRequest2 = new UploadDocumentRequest
            {
                DocumentName = document2.DocumentName,
                Description = document2.Description,
                Category = document2.Category,
                DocumentContent = documentContent2.Content,
                Groups = new List<UserGroup> { userGroup }
            };
            var document3 = new DocumentMetadata { DocumentName = "docName3", Description = "docDesc3", Category = DocumentCategories.PDF };
            var documentContent3 = new DocumentContent { Content = new byte[] { 23, 3, 2, 3, 54, 3, 23 } };
            var newDocumentRequest3 = new UploadDocumentRequest
            {
                DocumentName = document3.DocumentName,
                Description = document3.Description,
                Category = document3.Category,
                DocumentContent = documentContent3.Content,
                Users = new List<User> { user5 }
            };
            var updatedDocument3 = new DocumentMetadata { DocumentName = "udocName3", Description = "udocDesc3", Category = DocumentCategories.DOCX };
            var updateDocumentRequest = new UpdateDocumentRequest
            {
                DocumentName = updatedDocument3.DocumentName,
                Description = updatedDocument3.Description,
                Category = updatedDocument3.Category,
                Users = new List<User> { user3 }
            };
            var searchCriteria1 = new DocumentSearchCriteria { DocumentName = "docName" };
            var searchCriteria2 = new DocumentSearchCriteria { Category = DocumentCategories.PDF };

            var adminUserInfo = new UserInfo { UserName = user.UserName, Password = "password" };
            var user2UserInfo = new UserInfo { UserName = user2.UserName, Password = "aA1$aaaa2" };
            var user3UserInfo = new UserInfo { UserName = user3.UserName, Password = "aA1$aaaa3" };
            var user4UserInfo = new UserInfo { UserName = user4.UserName, Password = "aA1$aaaa4" };
            var user5UserInfo = new UserInfo { UserName = user5.UserName, Password = "aA1$aaaa5" };

            await LoginAsync(adminUserInfo);

            var createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest1);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user2);

            createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest2);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user3);

            createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest3);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user4);

            createResponse = await _client.PostAsJsonAsync("/api/users", newUserRequest4);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            userResult = await createResponse.Content.ReadAsJsonAsync<User>();
            userResult.Should().BeEquivalentTo(user5);

            createResponse = await _client.PostAsJsonAsync("/api/groups", newUserGroupRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var userGroupResult = await createResponse.Content.ReadAsJsonAsync<UserGroup>();
            userGroup.UserGroupId = userGroupResult.UserGroupId;
            userGroupResult.Should().BeEquivalentTo(userGroup);

            createResponse = await _client.PostAsJsonAsync("/api/documents", newDocumentRequest1);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdocumentResult = await createResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            document1.DocumentId = createdocumentResult.DocumentId;
            document1.UploadTimestamp = createdocumentResult.UploadTimestamp;
            documentContent1.MetadataId = createdocumentResult.DocumentId;
            createdocumentResult.Should().BeEquivalentTo(document1);

            createResponse = await _client.PostAsJsonAsync("/api/documents", newDocumentRequest2);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createdocumentResult = await createResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            document2.DocumentId = createdocumentResult.DocumentId;
            document2.UploadTimestamp = createdocumentResult.UploadTimestamp;
            documentContent2.MetadataId = createdocumentResult.DocumentId;
            createdocumentResult.Should().BeEquivalentTo(document2);

            await LoginAsync(user2UserInfo);
            createResponse = await _client.PostAsJsonAsync("/api/documents", newDocumentRequest3);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            await LoginAsync(user3UserInfo);
            createResponse = await _client.PostAsJsonAsync("/api/documents", newDocumentRequest3);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createdocumentResult = await createResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            document3.DocumentId = createdocumentResult.DocumentId;
            document3.UploadTimestamp = createdocumentResult.UploadTimestamp;
            updatedDocument3.DocumentId = createdocumentResult.DocumentId;
            updatedDocument3.UploadTimestamp = createdocumentResult.UploadTimestamp;
            documentContent3.MetadataId = createdocumentResult.DocumentId;
            createdocumentResult.Should().BeEquivalentTo(document3);

            var getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResult = await getResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            getResult.Should().BeEquivalentTo(document1);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var contentResult = await getResponse.Content.ReadAsJsonAsync<DocumentContent>();
            contentResult.Content.Should().BeEquivalentTo(documentContent1.Content);

            await LoginAsync(user5UserInfo);
            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            var updateResponse = await _client.PutAsJsonAsync($"/api/documents/{document3.DocumentId}", updateDocumentRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedUserGroupResult = await updateResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            updatedUserGroupResult.Should().BeEquivalentTo(updatedDocument3);

            updateResponse = await _client.PutAsJsonAsync($"/api/documents/{document3.DocumentId}", updateDocumentRequest);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            getResponse = await _client.GetAsync($"/api/documents");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var getResults = await getResponse.Content.ReadAsJsonAsync<List<DocumentMetadata>>();
            getResults.Count.Should().Be(0);

            var deleteResponse = await _client.DeleteAsync($"/api/documents/{document3.DocumentId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            await LoginAsync(user3UserInfo);

            getResponse = await _client.GetAsync($"/api/documents");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResults = await getResponse.Content.ReadAsJsonAsync<List<DocumentMetadata>>();
            getResults.Count.Should().Be(3);

            var searchResponse = await _client.PostAsJsonAsync("/api/documents/search", searchCriteria1);
            searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var searchdocumentResult = await searchResponse.Content.ReadAsJsonAsync<List<DocumentMetadata>>();
            searchdocumentResult.Count.Should().Be(1);
            searchdocumentResult[0].Should().BeEquivalentTo(document1);

            deleteResponse = await _client.DeleteAsync($"/api/documents/{document3.DocumentId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            await LoginAsync(user4UserInfo);

            getResponse = await _client.GetAsync($"/api/documents");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResults = await getResponse.Content.ReadAsJsonAsync<List<DocumentMetadata>>();
            getResults.Count.Should().Be(1);

            searchResponse = await _client.PostAsJsonAsync("/api/documents/search", searchCriteria2);
            searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            searchdocumentResult = await searchResponse.Content.ReadAsJsonAsync<List<DocumentMetadata>>();
            searchdocumentResult.Count.Should().Be(1);
            searchdocumentResult[0].Should().BeEquivalentTo(document2);

            await LoginAsync(user2UserInfo);

            deleteResponse = await _client.DeleteAsync($"/api/documents/{document2.DocumentId}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            getResponse = await _client.GetAsync($"/api/documents/{document2.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResult = await getResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            getResult.Should().BeEquivalentTo(document2);

            getResponse = await _client.GetAsync($"/api/documents/{document2.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            contentResult = await getResponse.Content.ReadAsJsonAsync<DocumentContent>();
            contentResult.Content.Should().BeEquivalentTo(documentContent2.Content);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResult = await getResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            getResult.Should().BeEquivalentTo(document1);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            contentResult = await getResponse.Content.ReadAsJsonAsync<DocumentContent>();
            contentResult.Content.Should().BeEquivalentTo(documentContent1.Content);

            await LoginAsync(user4UserInfo);
            getResponse = await _client.GetAsync($"/api/documents/{document2.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            getResponse = await _client.GetAsync($"/api/documents/{document2.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getResult = await getResponse.Content.ReadAsJsonAsync<DocumentMetadata>();
            getResult.Should().BeEquivalentTo(document1);

            getResponse = await _client.GetAsync($"/api/documents/{document1.DocumentId}/content");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            contentResult = await getResponse.Content.ReadAsJsonAsync<DocumentContent>();
            contentResult.Content.Should().BeEquivalentTo(documentContent1.Content);
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
