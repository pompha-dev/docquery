using OpenAI;
using OpenAI.Embeddings;

namespace RagApp.Api.Services
{
    public class EmbeddingService
    {
        private readonly OpenAIClient _client;
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(OpenAIClient client)
        {
            _client = client;
            _embeddingClient = client.GetEmbeddingClient("text-embedding-3-small");
        }

        public async Task<ReadOnlyMemory<float>> GenerateEmbeddingAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be empty", nameof(text));
            }

            var response = await _embeddingClient.GenerateEmbeddingsAsync(new[] { text });

            var embedding = response.Value[0].ToFloats();

            return embedding;

        }
    }
}
