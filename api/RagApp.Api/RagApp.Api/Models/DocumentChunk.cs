namespace RagApp.Api.Models
{
    public class DocumentChunk
    {
        public int Id { get; set; }

        public int DocumentId { get; set; }

        public int ChunkIndex { get; set; }

        public string Text { get; set; } = "";

        public string EmbeddingJson { get; set; } = "";

        public Document Document { get; set; } = null!;
    }
}
