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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroup>>> GetGroups()
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _groupService.GetGroupsAsync();
                if (result == null)
                    return NoContent();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroup>> GetGroup(int id)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _groupService.GetGroupAsync(id);
                if (result == null)
                    return NoContent();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UserGroup>> CreateGroup([FromBody] CreateUserGroupRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _groupService.CreateGroupAsync(request);
                if (result == null)
                    return BadRequest();
                return Created(new Uri($"/api/groups/{result.UserGroupId}", UriKind.Relative), result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserGroup>> UpdateGroup(int id, [FromBody] UpdateUserGroupRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _groupService.UpdateGroupAsync(id, request);
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
        public async Task<IActionResult> DeleteGroup(int id)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Admin))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                await _groupService.DeleteGroupAsync(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
