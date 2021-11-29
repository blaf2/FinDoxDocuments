using FinDoxDocumentsAPI.Handlers;
using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            var result = await _userService.GetUsersAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            var result = await _userService.GetUserAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _userService.CreateUserAsync(request);
                if (result == null)
                    return BadRequest();
                return Created(new Uri($"/api/users/{result.UserId}", UriKind.Relative), result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _userService.UpdateUserAsync(id, request);
                if (result == null)
                    return BadRequest();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                await _userService.DeleteUserAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
