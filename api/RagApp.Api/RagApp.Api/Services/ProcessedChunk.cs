namespace RagApp.Api.Services
{
    public class ProcessedChunk
    {
        public required string Text { get; set; }
        public required ReadOnlyMemory<float> Embedding { get; set; }


    }
}
