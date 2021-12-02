using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return Ok(await _userService.GetUserAsync(id));
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _userService.CreateUserAsync(request);
            return Created(new Uri($"/api/users/{result?.UserId}", UriKind.Relative), result);
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            request.Id = id;
            return Ok(await _userService.UpdateUserAsync(request));
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
