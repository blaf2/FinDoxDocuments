using FinDoxDocumentsAPI.Controllers;
using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPIUnitTests
{
    [TestClass]
    public class DocumentControllerTests
    {
        [TestMethod]
        public async Task GetDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var document1 = new DocumentMetadata { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, UploadTimestamp = DateTime.Now };
            var document2 = new DocumentMetadata { DocumentId = 2, DocumentName = "doc2", Description = "des21", Category = DocumentCategories.PDF, UploadTimestamp = DateTime.Now };
            var documents = new List<DocumentMetadata> { document1, document2 };

            documentServiceMock.Setup(x => x.GetDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.GetDocument(1);

            documentServiceMock.Verify(x => x.GetDocumentAsync(1, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.GetDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext.Items["User"] = user2;

            result = await target.GetDocument(1);

            documentServiceMock.Verify(x => x.GetDocumentAsync(1, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.GetUserDocumentsAsync(user3.UserId)).Returns(Task.FromResult(documents as IEnumerable<DocumentMetadata>));

            target.ControllerContext.HttpContext.Items["User"] = user3;

            var results = await target.GetUserDocuments();

            documentServiceMock.Verify(x => x.GetUserDocumentsAsync(user3.UserId), Times.Once);

            Assert.IsTrue(results?.Result is OkObjectResult);
            okResult = results.Result as OkObjectResult;
            Assert.AreEqual(documents, okResult.Value);
        }

        [TestMethod]
        public async Task DownloadDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };

            var document1 = new DocumentMetadata { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, UploadTimestamp = DateTime.Now };
            var document2 = new DocumentMetadata { DocumentId = 2, DocumentName = "doc2", Description = "des21", Category = DocumentCategories.PDF, UploadTimestamp = DateTime.Now };
            var documentsResult = new List<DocumentMetadata> { document1 };

            var searchRequest = new DocumentSearchCriteria { DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG };

            documentServiceMock.Setup(x => x.SearchDocumentsAsync(searchRequest, It.IsAny<User>())).Returns(Task.FromResult(documentsResult as IEnumerable<DocumentMetadata>));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.SearchDocuments(searchRequest);

            documentServiceMock.Verify(x => x.SearchDocumentsAsync(searchRequest, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(documentsResult, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.SearchDocumentsAsync(searchRequest, It.IsAny<User>())).Returns(Task.FromResult(documentsResult as IEnumerable<DocumentMetadata>));

            target.ControllerContext.HttpContext.Items["User"] = user2;

            result = await target.SearchDocuments(searchRequest);

            documentServiceMock.Verify(x => x.SearchDocumentsAsync(searchRequest, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(documentsResult, okResult.Value);
        }

        [TestMethod]
        public async Task SearchDocumentTest()
        {
            var documentServiceMock = new Mock<IDocumentService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };

            var documentContent1 = new DocumentContent { Id = 1, MetadataId = 10, Content = new byte[] { 23, 24, 32, 1, 35, 2, 32, } };

            documentServiceMock.Setup(x => x.DownloadDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(documentContent1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.DownloadDocument(1);

            documentServiceMock.Verify(x => x.DownloadDocumentAsync(1, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(documentContent1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.DownloadDocumentAsync(1, It.IsAny<User>())).Returns(Task.FromResult(documentContent1));

            target.ControllerContext.HttpContext.Items["User"] = user2;

            result = await target.DownloadDocument(1);

            documentServiceMock.Verify(x => x.DownloadDocumentAsync(1, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(documentContent1, okResult.Value);
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
            var document1 = new DocumentMetadata { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, UploadTimestamp = DateTime.Now };

            var uploadDocumentRequest = new UploadDocumentRequest { DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, DocumentContent = new byte[] { 23, 34, 32, 32, 54, 23, 23 }, Users = users, Groups = groups };

            documentServiceMock.Setup(x => x.UploadDocumenAsync(uploadDocumentRequest)).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.UploadDocument(uploadDocumentRequest);

            documentServiceMock.Verify(x => x.UploadDocumenAsync(uploadDocumentRequest), Times.Once);

            Assert.IsTrue(result?.Result is CreatedResult);
            var createdResult = result.Result as CreatedResult;
            Assert.AreEqual(document1, createdResult.Value);
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
            var document1 = new DocumentMetadata { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, UploadTimestamp = DateTime.Now };

            var updateDocumentRequest = new UpdateDocumentRequest { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, Users = users, Groups = groups };

            documentServiceMock.Setup(x => x.UpdateDocumentAsync(updateDocumentRequest, It.IsAny<User>())).Returns(Task.FromResult(document1));

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.UpdateDocument(1, updateDocumentRequest);

            documentServiceMock.Verify(x => x.UpdateDocumentAsync(updateDocumentRequest, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.UpdateDocumentAsync(updateDocumentRequest, It.IsAny<User>())).Returns(Task.FromResult(document1));

            target.ControllerContext.HttpContext.Items["User"] = user2;

            result = await target.UpdateDocument(1, updateDocumentRequest);

            documentServiceMock.Verify(x => x.UpdateDocumentAsync(updateDocumentRequest, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            okResult = result.Result as OkObjectResult;
            Assert.AreEqual(document1, okResult.Value);
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
            var document1 = new DocumentMetadata { DocumentId = 1, DocumentName = "doc1", Description = "desc1", Category = DocumentCategories.PNG, UploadTimestamp = DateTime.Now };

            documentServiceMock.Setup(x => x.DeleteDocumentAsync(1, It.IsAny<User>())).Returns(Task.CompletedTask);

            var target = new DocumentsController(documentServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext();
            target.ControllerContext.HttpContext.Items.Add("User", user1);

            var result = await target.DeleteDocument(1);

            documentServiceMock.Verify(x => x.DeleteDocumentAsync(1, It.Is<User>(x => x.UserId == user1.UserId && x.UserName == user1.UserName && x.UserType == user1.UserType)), Times.Once);

            Assert.IsTrue(result is OkResult);

            documentServiceMock.Reset();

            documentServiceMock.Setup(x => x.DeleteDocumentAsync(1, It.IsAny<User>())).Returns(Task.CompletedTask);

            target.ControllerContext.HttpContext.Items["User"] = user2;

            result = await target.DeleteDocument(1);

            documentServiceMock.Verify(x => x.DeleteDocumentAsync(1, It.Is<User>(x => x.UserId == user2.UserId && x.UserName == user2.UserName && x.UserType == user2.UserType)), Times.Once);

            Assert.IsTrue(result is OkResult);
        }
    }
}
