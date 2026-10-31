namespace RagApp.Api.Models
{
    public class ChatRequest
    {
        public int DocumentId { get; set; }
        public string Question { get; set; } = "";
    }
}
