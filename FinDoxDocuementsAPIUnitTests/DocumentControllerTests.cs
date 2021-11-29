using FinDoxDocumentsAPI.Controllers;
using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPIUnitTests
{
    [TestClass]
    public class DocumentControllerTests
    {
        [TestMethod]
        public async Task DownloadDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var document1 = new Document { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, UploadTimestamp = DateTime.Now };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user1.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user1.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user2.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user2.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user3.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user3.UserId.ToString())
                                   }, "TestAuth"));

            documentServiceMock.Setup(x => x.DownloadDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.DownloadDocument(1);

            documentServiceMock.Verify(x => x.DownloadDocumentAsync(1, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.DownloadDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.DownloadDocument(1);

            documentServiceMock.Verify(x => x.DownloadDocumentAsync(1, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.DownloadDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.DownloadDocument(1);

            documentServiceMock.Verify(x => x.DownloadDocumentAsync(1, It.Is<User>(x => x.UserId == user3.UserId && x.UserName == user3.UserName && x.UserType == user3.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);
        }

        [TestMethod]
        public async Task UploadDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var document1 = new Document { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, UploadTimestamp = DateTime.Now };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user1.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user1.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user2.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user2.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user3.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user3.UserId.ToString())
                                   }, "TestAuth"));

            var uploadDocumentRequest = new UploadDocumentRequest { DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, Users = users, Groups = groups };

            documentServiceMock.Setup(x => x.UploadDocumenAsync(uploadDocumentRequest)).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.UploadDocument(uploadDocumentRequest);

            documentServiceMock.Verify(x => x.UploadDocumenAsync(uploadDocumentRequest), Times.Once);

            Assert.IsTrue(result?.Result is CreatedResult);
            var createdResult = result.Result as CreatedResult;
            Assert.AreEqual(document1, createdResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.UploadDocumenAsync(uploadDocumentRequest)).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.UploadDocument(uploadDocumentRequest);

            documentServiceMock.Verify(x => x.UploadDocumenAsync(uploadDocumentRequest), Times.Once);

            Assert.IsTrue(result?.Result is CreatedResult);
            createdResult = result.Result as CreatedResult;
            Assert.AreEqual(document1, createdResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.UploadDocument(uploadDocumentRequest);

            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task UpdateDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var document1 = new Document { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, UploadTimestamp = DateTime.Now };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user1.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user1.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user2.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user2.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user3.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user3.UserId.ToString())
                                   }, "TestAuth"));

            var updateDocumentRequest = new UpdateDocumentRequest { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, Users = users, Groups = groups };

            documentServiceMock.Setup(x => x.UpdateDocumentAsync(1, updateDocumentRequest, It.IsAny<User>())).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.UpdateDocument(1, updateDocumentRequest);

            documentServiceMock.Verify(x => x.UpdateDocumentAsync(1, updateDocumentRequest, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.UpdateDocumentAsync(1, updateDocumentRequest, It.IsAny<User>())).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.UpdateDocument(1, updateDocumentRequest);

            documentServiceMock.Verify(x => x.UpdateDocumentAsync(1, updateDocumentRequest, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.UpdateDocument(1, updateDocumentRequest);

            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task DeleteDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var document1 = new Document { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, UploadTimestamp = DateTime.Now };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user1.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user1.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user2.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user2.UserId.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString()),
                                        new Claim(nameof(User.UserName),  user3.UserName.ToString()),
                                        new Claim(nameof(User.UserId),  user3.UserId.ToString())
                                   }, "TestAuth"));

            documentServiceMock.Setup(x => x.DeleteDocumentAsync(1, It.IsAny<User>())).Returns(Task.CompletedTask);

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.DeleteDocument(1);

            documentServiceMock.Verify(x => x.DeleteDocumentAsync(1, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result is OkResult);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.DeleteDocumentAsync(1, It.IsAny<User>())).Returns(Task.CompletedTask);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.DeleteDocument(1);

            documentServiceMock.Verify(x => x.DeleteDocumentAsync(1, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result is OkResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.DeleteDocument(1);

            Assert.IsTrue(result is UnauthorizedObjectResult);
        }
    }
}
