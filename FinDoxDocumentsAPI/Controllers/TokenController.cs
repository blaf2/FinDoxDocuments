using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace FinDoxDocumentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public TokenController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserInfo userInfo)
        {
            if (userInfo != null && userInfo.UserName != null && userInfo.Password != null)
            {
                var user = await GetUserFromCredientials(userInfo);

                if (user != null)
                {
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(nameof(Models.User.UserId), user.UserId.ToString()),
                    new Claim(nameof(Models.User.UserName), user.UserName),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<User> GetUserFromCredientials(UserInfo userInfo)
        {
            return await _userService.GetUserFromCredientialsAsync(userInfo);
        }
    }
}
