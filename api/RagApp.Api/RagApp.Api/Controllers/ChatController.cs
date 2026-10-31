using Microsoft.AspNetCore.Mvc;
using RagApp.Api.Services;
using RagApp.Api.Models;
using RagApp.Api.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace RagApp.Api.Controllers
{
    [ApiController]
    [Route("Chat")]
    public class ChatController : ControllerBase
    {
        private readonly EmbeddingService _embeddingService;
        private readonly DocumentRetrievalService _documentRetrievalService;
        private readonly ChatService _chatService;
        private readonly ApplicationDbContext _context;

        public ChatController(EmbeddingService embeddingService, DocumentRetrievalService documentRetrievalService, 
                              ChatService chatService, ApplicationDbContext context )
        {
            _embeddingService = embeddingService;
            _documentRetrievalService = documentRetrievalService;
            _chatService = chatService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AskQuestion([FromBody] ChatRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var document = await _context.Documents.FirstOrDefaultAsync(x => x.Id == request.DocumentId &&
                                                                   x.UserId == userId);
            if (document == null)
            {
                return NotFound("Document not found.");
            }
            var questionEmbedding = await _embeddingService
                                           .GenerateEmbeddingAsync(request.Question);
            var relevantChunks = await _documentRetrievalService
                                        .RetrieveRelevantChunksAsync
                                        (questionEmbedding, request.DocumentId);
            var answer = await _chatService.GenerateAnswerAsync(
                                request.Question,
                                relevantChunks);
            return Ok(new {answer});
        }
    }
}
