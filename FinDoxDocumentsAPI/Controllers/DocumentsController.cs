using FinDoxDocumentsAPI.Handlers;
using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FinDoxDocumentsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Document>> DownloadDocument(int id)
        {
            var user = PermissionsHandler.GetUser(User.Identity as ClaimsIdentity);
            if (user == null)
                return BadRequest(PermissionsHandler.UserDoesNotExistError);
            try
            {
                var result = await _documentService.DownloadDocumentAsync(id, user);
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
        [HttpPut("{id}")]
        public async Task<ActionResult<Document>> UpdateDocument(int id, [FromBody] UpdateDocumentRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Manager))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            var user = PermissionsHandler.GetUser(User.Identity as ClaimsIdentity);
            if (user == null)
                return BadRequest(PermissionsHandler.UserDoesNotExistError);
            try
            {
                var result = await _documentService.UpdateDocumentAsync(id, request, user);
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
        [HttpPost]
        public async Task<ActionResult<Document>> UploadDocument([FromBody] UploadDocumentRequest request)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Manager))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            try
            {
                var result = await _documentService.UploadDocumenAsync(request);
                if (result == null)
                    return BadRequest();
                return Created(new Uri($"/api/documents/{result.DocumentId}", UriKind.Relative), result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            if (!PermissionsHandler.HasPermission(User.Identity as ClaimsIdentity, UserTypes.Manager))
                return Unauthorized(PermissionsHandler.UserPermissionError);
            var user = PermissionsHandler.GetUser(User.Identity as ClaimsIdentity);
            if (user == null)
                return BadRequest(PermissionsHandler.UserDoesNotExistError);
            try
            {
                await _documentService.DeleteDocumentAsync(id, user);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
