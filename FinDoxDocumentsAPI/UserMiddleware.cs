using FinDoxDocumentsAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI
{
    public class UserMiddleware
    {
        private const string UserDoesNotExistError = "User does not exist";
        private readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var user = GetUser(httpContext.User.Identity as ClaimsIdentity);
            if (user == null)
            {
                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                await httpContext.Response.WriteAsync(UserDoesNotExistError);
                return;
            }
            httpContext.Items.Add("User", user);
            await _next(httpContext);
        }

        private User GetUser(ClaimsIdentity claimsIdentity)
        {
            if (!int.TryParse(claimsIdentity?.FindFirst(nameof(User.UserId))?.Value, out int userId))
                return null;

            var userName = claimsIdentity?.FindFirst(nameof(User.UserName))?.Value;
            if (string.IsNullOrEmpty(userName))
                return null;

            if (!Enum.TryParse(claimsIdentity?.FindFirst(ClaimTypes.Role)?.Value, out UserTypes userType))
                return null;

            return new User { UserId = userId, UserName = userName, UserType = userType };
        }
    }

    public static class UserMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserMiddleware>();
        }
    }

}
