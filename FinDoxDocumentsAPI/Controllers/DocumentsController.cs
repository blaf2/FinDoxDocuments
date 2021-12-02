using FinDoxDocumentsAPI.Models;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<DocumentMetadata>> GetDocument(int id)
        {
            return Ok(await _documentService.GetDocumentAsync(id, HttpContext.Items["User"] as User));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentMetadata>>> GetUserDocuments()
        {
            var user = HttpContext.Items["User"] as User;
            return Ok(await _documentService.GetUserDocumentsAsync(user.UserId));
        }

        [Authorize]
        [HttpGet("{id}/content")]
        public async Task<ActionResult<DocumentContent>> DownloadDocument(int id)
        {
            return Ok(await _documentService.DownloadDocumentAsync(id, HttpContext.Items["User"] as User));
        }

        [Authorize]
        [HttpPost("search")]
        public async Task<ActionResult<IEnumerable<DocumentMetadata>>> SearchDocuments([FromBody] DocumentSearchCriteria criteria)
        {
            return Ok(await _documentService.SearchDocumentsAsync(criteria, HttpContext.Items["User"] as User));
        }

        [RoleAuthorize(Roles.Admin, Roles.Manager)]
        [HttpPut("{id}")]
        public async Task<ActionResult<DocumentMetadata>> UpdateDocument(int id, [FromBody] UpdateDocumentRequest request)
        {
            request.DocumentId = id;
            return Ok(await _documentService.UpdateDocumentAsync(request, HttpContext.Items["User"] as User));
        }

        [RoleAuthorize(Roles.Admin, Roles.Manager)]
        [HttpPost]
        public async Task<ActionResult<DocumentMetadata>> UploadDocument([FromBody] UploadDocumentRequest request)
        {
            var result = await _documentService.UploadDocumenAsync(request);
            return Created(new Uri($"/api/documents/{result?.DocumentId}", UriKind.Relative), result);
        }

        [RoleAuthorize(Roles.Admin, Roles.Manager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            await _documentService.DeleteDocumentAsync(id, HttpContext.Items["User"] as User);
            return Ok();
        }
    }
}
