using Microsoft.EntityFrameworkCore;
using RagApp.Api.Data;
using RagApp.Api.Models;
using System.Text.Json;

namespace RagApp.Api.Services
{
    public class DocumentRetrievalService
    {
        private readonly ApplicationDbContext _context;
        private readonly CosineSimilarityService _similarityService;
        public DocumentRetrievalService(ApplicationDbContext context, CosineSimilarityService similarityService) 
        { 
            _context = context;
            _similarityService = similarityService;
        }

        public async Task<List<DocumentChunk>> RetrieveRelevantChunksAsync(
                                                    ReadOnlyMemory<float> questionEmbedding,
                                                    int documentId)
        {
            var relevantChunks = await _context.DocumentChunks.Where(chunk => chunk.DocumentId == documentId).ToListAsync();
            var scoredChunks = new List<(DocumentChunk Chunk, double Similarity)>();
            foreach (var chunk in relevantChunks)
            {
                var embedding = JsonSerializer.Deserialize<float[]>(chunk.EmbeddingJson);
                if (embedding is null)
                {
                    throw new InvalidOperationException(
                        $"Could not deserialize embedding for chunk {chunk.Id}.");
                }
                var similarity = _similarityService.Calculate(questionEmbedding, embedding);
                scoredChunks.Add((chunk, similarity));
            }

            var topChunks = scoredChunks.OrderByDescending(x => x.Similarity)
                                        .Take(5)
                                        .Select(x => x.Chunk)
                                        .ToList();
            return topChunks;
        }
    }
}
