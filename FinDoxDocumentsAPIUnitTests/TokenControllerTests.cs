using FinDoxDocumentsAPI.Controllers;
using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPIUnitTests
{
    [TestClass]
    public class TokenControllerTests
    {
        [TestMethod]
        public async Task GetTokenTest()
        {
            var userServiceMock = new Mock<IUserService>();
            var configMock = new Mock<IConfiguration>();
            var user1 = new User() { UserId = 1, UserName = "name1", UserType = UserTypes.Admin };
            var user2 = new User() { UserId = 2, UserName = "name2", UserType = UserTypes.Manager };
            var user3 = new User() { UserId = 3, UserName = "name3", UserType = UserTypes.Regular };
            var users = new List<User> { user1, user2 };
            var userInfo1 = new UserInfo() { UserName = user1.UserName, Password = "password" };

            userServiceMock.Setup(x => x.GetUserFromCredientialsAsync(userInfo1)).Returns(Task.FromResult(user1));
            configMock.Setup(x => x["Jwt:Subject"]).Returns("Subject");
            configMock.Setup(x => x["Jwt:Key"]).Returns("dsfj33jnleifoefnweo23inosifn3io2nkn");
            configMock.Setup(x => x["Jwt:Issuer"]).Returns("Issuer");
            configMock.Setup(x => x["Jwt:Audience"]).Returns("Audience");

            var target = new TokenController(configMock.Object, userServiceMock.Object);

            var result = await target.Post(userInfo1);

            userServiceMock.Verify(x => x.GetUserFromCredientialsAsync(userInfo1), Times.Once);

            Assert.IsTrue(result is OkObjectResult);


            userServiceMock.Reset();

            userServiceMock.Setup(x => x.GetUserFromCredientialsAsync(userInfo1)).Returns(Task.FromResult((User)null));

            result = await target.Post(userInfo1);

            userServiceMock.Verify(x => x.GetUserFromCredientialsAsync(userInfo1), Times.Once);

            Assert.IsTrue(result is BadRequestObjectResult);
        }
    }
}
