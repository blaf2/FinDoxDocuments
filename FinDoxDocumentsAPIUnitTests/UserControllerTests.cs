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
    public class UserControllerTests
    {
        [TestMethod]
        public async Task GetAllUsersTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin};
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2, user3 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            userServiceMock.Setup(x => x.GetUsersAsync()).Returns(Task.FromResult(users as IEnumerable<User>));

            var target = new UsersController(userServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.GetUsers();

            userServiceMock.Verify(x => x.GetUsersAsync(), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(users, okResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.GetUsers();
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.GetUsers();
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task GetUserTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser3 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user3.UserType.ToString())
                                   }, "TestAuth"));

            userServiceMock.Setup(x => x.GetUserAsync(2)).Returns(Task.FromResult(user2));

            var target = new UsersController(userServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.GetUser(2);

            userServiceMock.Verify(x => x.GetUserAsync(2), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(user2, okResult.Value);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.GetUser(1);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);

            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser3 };

            result = await target.GetUser(3);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task CreateUserTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));

            var createUserRequest = new CreateUserRequest { UserName = "name3", Password = "password", UserType = UserTypes.Regular };

            userServiceMock.Setup(x => x.CreateUserAsync(createUserRequest)).Returns(Task.FromResult(user3));

            var target = new UsersController(userServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.CreateUser(createUserRequest);

            userServiceMock.Verify(x => x.CreateUserAsync(createUserRequest), Times.Once);

            Assert.IsTrue(result?.Result is CreatedResult);
            var createdResult = result.Result as CreatedResult;
            Assert.AreEqual(user3, createdResult.Value);

            userServiceMock.Reset();

            userServiceMock.Setup(x => x.CreateUserAsync(createUserRequest)).Throws(new InvalidOperationException());

            result = await target.CreateUser(createUserRequest);

            userServiceMock.Verify(x => x.CreateUserAsync(createUserRequest), Times.Once);

            Assert.IsTrue(result?.Result is BadRequestObjectResult);


            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.CreateUser(createUserRequest);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task UpdateUserTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));

            var updateUserRequest = new UpdateUserRequest { Id = 3, UserName = "name3", Password = "password", UserType = UserTypes.Regular };

            userServiceMock.Setup(x => x.UpdateUserAsync(3, updateUserRequest)).Returns(Task.FromResult(user3));

            var target = new UsersController(userServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.UpdateUser(3, updateUserRequest);

            userServiceMock.Verify(x => x.UpdateUserAsync(3, updateUserRequest), Times.Once);

            Assert.IsTrue(result?.Result is OkObjectResult);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(user3, okResult.Value);

            userServiceMock.Reset();

            userServiceMock.Setup(x => x.UpdateUserAsync(3, updateUserRequest)).Throws(new InvalidOperationException());

            result = await target.UpdateUser(3, updateUserRequest);

            userServiceMock.Verify(x => x.UpdateUserAsync(3, updateUserRequest), Times.Once);

            Assert.IsTrue(result?.Result is BadRequestObjectResult);


            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.UpdateUser(3, updateUserRequest);
            Assert.IsTrue(result?.Result is UnauthorizedObjectResult);
        }

        [TestMethod]
        public async Task DeleteUserTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2 };
            var httpUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user1.UserType.ToString())
                                   }, "TestAuth"));
            var httpUser2 = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(nameof(User.UserType),  user2.UserType.ToString())
                                   }, "TestAuth"));

            userServiceMock.Setup(x => x.DeleteUserAsync(3)).Returns(Task.CompletedTask);

            var target = new UsersController(userServiceMock.Object);
            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser };

            var result = await target.DeleteUser(3);

            userServiceMock.Verify(x => x.DeleteUserAsync(3), Times.Once);

            Assert.IsTrue(result is OkResult);

            userServiceMock.Reset();

            userServiceMock.Setup(x => x.DeleteUserAsync(3)).Throws(new InvalidOperationException());

            result = await target.DeleteUser(3);

            userServiceMock.Verify(x => x.DeleteUserAsync(3), Times.Once);

            Assert.IsTrue(result is BadRequestObjectResult);


            target.ControllerContext.HttpContext = new DefaultHttpContext { User = httpUser2 };

            result = await target.DeleteUser(3);
            Assert.IsTrue(result is UnauthorizedObjectResult);
        }
    }
}
