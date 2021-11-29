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
    public class GroupControllerTests
    {
        [TestMethod]
        public async Task GetAllGroupsTest()
        {
            var groupServiceMock = new Mock<IGroupService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            groupServiceMock.Setup(x => x.GetGroupsAsync()).Returns(Task.FromResult(groups as IEnumerable<UserGroup>));

            var target = new GroupsController(groupServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.GetGroups();

            groupServiceMock.Verify(x => x.GetGroupsAsync(), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(groups, okResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.GetGroups();
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.GetGroups();
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task GetGroupTest()
        {
            var groupServiceMock = new Mock<IGroupService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var groups = new List<UserGroup> { group1, group2 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            groupServiceMock.Setup(x => x.GetGroupAsync(2)).Returns(Task.FromResult(group2));

            var target = new GroupsController(groupServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.GetGroup(2);

            groupServiceMock.Verify(x => x.GetGroupAsync(2), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(group2, okResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.GetGroup(2);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.GetGroup(2);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task CreateGroupTest()
        {
            var groupServiceMock = new Mock<IGroupService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            var createGroupRequest = new CreateUserGroupRequest { Name = "group2", Members = new List<User> { user2, user3 } };

            groupServiceMock.Setup(x => x.CreateGroupAsync(createGroupRequest)).Returns(Task.FromResult(group2));

            var target = new GroupsController(groupServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.CreateGroup(createGroupRequest);

            groupServiceMock.Verify(x => x.CreateGroupAsync(createGroupRequest), Times.Once);

            Assert.IsTrue(result?.Result is CreatedResult);
            var createdResult = result.Result as CreatedResult;
            Assert.AreEqual(group2, createdResult.Value);

            groupServiceMock.Reset();

            groupServiceMock.Setup(x => x.CreateGroupAsync(createGroupRequest)).Throws(new InvalidOperationException());

            result = await target.CreateGroup(createGroupRequest);

            groupServiceMock.Verify(x => x.CreateGroupAsync(createGroupRequest), Times.Once);

            Assert.IsTrue(result?.Result is BadRequestObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.CreateGroup(createGroupRequest);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task UpdateGroupTest()
        {
            var groupServiceMock = new Mock<IGroupService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            var updateGroupRequest = new UpdateUserGroupRequest { Id = 2, Name = "group2", Members = new List<User> { user2, user3 } };

            groupServiceMock.Setup(x => x.UpdateGroupAsync(2, updateGroupRequest)).Returns(Task.FromResult(group2));

            var target = new GroupsController(groupServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.UpdateGroup(2, updateGroupRequest);

            groupServiceMock.Verify(x => x.UpdateGroupAsync(2, updateGroupRequest), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(group2, okResult.Value);

            groupServiceMock.Reset();

            groupServiceMock.Setup(x => x.UpdateGroupAsync(2, updateGroupRequest)).Throws(new InvalidOperationException());

            result = await target.UpdateGroup(2, updateGroupRequest);

            groupServiceMock.Verify(x => x.UpdateGroupAsync(2, updateGroupRequest), Times.Once);

            Assert.IsTrue(result?.Result is BadRequestObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.UpdateGroup(2, updateGroupRequest);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task DeleteGroupTest()
        {
            var groupServiceMock = new Mock<IGroupService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var group1 = new UserGroup() { UserGroupId = 1, UserGroupName = "group1", Members = new List<User> { user1, user2 } };
            var group2 = new UserGroup() { UserGroupId = 2, UserGroupName = "group2", Members = new List<User> { user2, user3 } };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            groupServiceMock.Setup(x => x.DeleteGroupAsync(2)).Returns(Task.CompletedTask);

            var target = new GroupsController(groupServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.DeleteGroup(2);

            groupServiceMock.Verify(x => x.DeleteGroupAsync(2), Times.Once);

            Assert.IsTrue(result is OkResult);

            groupServiceMock.Reset();

            groupServiceMock.Setup(x => x.DeleteGroupAsync(2)).Throws(new InvalidOperationException());

            result = await target.DeleteGroup(2);

            groupServiceMock.Verify(x => x.DeleteGroupAsync(2), Times.Once);

            Assert.IsTrue(result is BadRequestObjectResult);


            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.DeleteGroup(2);
            Assert.IsTrue(result is UnauthorizedObjectResult);
        }
    }
}
