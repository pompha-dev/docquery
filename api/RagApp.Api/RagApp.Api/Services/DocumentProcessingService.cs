using RagApp.Api.Data;
using RagApp.Api.Models;
using System.Text.Json;

namespace RagApp.Api.Services
{
    public class DocumentProcessingService
    {
        private readonly DocumentTextExtractor _textExtractor;
        private readonly TextChunker _textChunker;
        private readonly EmbeddingService _embeddingService;
        private readonly ApplicationDbContext _context;

        public DocumentProcessingService(DocumentTextExtractor textExtractor, TextChunker textChunker, EmbeddingService embeddingService, ApplicationDbContext context)
        {
            _textExtractor = textExtractor;
            _textChunker = textChunker;
            _embeddingService = embeddingService;
            _context = context;
        }

        public async Task ProcessDocumentAsync(string filePath, int documentId)
        {
            var text = await _textExtractor.ExtractTextAsync(filePath);
            IReadOnlyList<string> chunks = _textChunker.Split(text);
            List<ProcessedChunk> processedChunks = new();

            foreach(var chunk in chunks)
            {
                var embedding = await _embeddingService.GenerateEmbeddingAsync(chunk);
                var processedChunk = new ProcessedChunk
                { 
                    Text = chunk,
                    Embedding = embedding
                };
                processedChunks.Add(processedChunk);
            }

            var result = processedChunks.Select((chunk, index) =>
                                                new DocumentChunk
                                                {
                                                    DocumentId = documentId,
                                                    ChunkIndex = index,
                                                    Text = chunk.Text,
                                                    EmbeddingJson = JsonSerializer.Serialize(chunk.Embedding.ToArray())
                                                });
            await _context.DocumentChunks.AddRangeAsync(result);
            await _context.SaveChangesAsync();
        }
    }
}
