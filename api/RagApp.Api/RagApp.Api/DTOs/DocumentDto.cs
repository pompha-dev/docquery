namespace RagApp.Api.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string FileName { get; set; } = "";
        public DateTime UploadedAt { get; set; }
    }
}
