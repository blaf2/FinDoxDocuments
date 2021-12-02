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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroup>>> GetGroups()
        {
            return Ok(await _groupService.GetGroupsAsync());
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroup>> GetGroup(int id)
        {
            return Ok(await _groupService.GetGroupAsync(id));
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<UserGroup>> CreateGroup([FromBody] CreateUserGroupRequest request)
        {
            var result = await _groupService.CreateGroupAsync(request);
            return Created(new Uri($"/api/groups/{result?.UserGroupId}", UriKind.Relative), result);
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserGroup>> UpdateGroup(int id, [FromBody] UpdateUserGroupRequest request)
        {
            request.Id = id;
            return Ok(await _groupService.UpdateGroupAsync(request));
        }

        [RoleAuthorize(Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            await _groupService.DeleteGroupAsync(id);
            return Ok();
        }
    }
}
