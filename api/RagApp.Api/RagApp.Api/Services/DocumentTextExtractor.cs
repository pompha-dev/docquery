
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using UglyToad.PdfPig;

namespace RagApp.Api.Services
{
    public class DocumentTextExtractor
    {
        public async Task<string> ExtractTextAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The document file was not found.",
                    filePath);
            }

            var extension = Path.GetExtension(filePath).ToLowerInvariant();

            return extension switch
            {
                ".pdf" => ExtractPdfText(filePath),
                ".docx" => ExtractDocxText(filePath),
                ".txt" => await File.ReadAllTextAsync(filePath, Encoding.UTF8),
                _ => throw new NotSupportedException(
                    $"The file format '{extension}' is not supported.")
            };
        }

        private static string ExtractPdfText(string filePath)
        {
            var text = new StringBuilder();

            using var document = PdfDocument.Open(filePath);

            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }

            return text.ToString();
        }

        private static string ExtractDocxText(string filePath)
        {
            using var document = WordprocessingDocument.Open(filePath, false);

            var body = document.MainDocumentPart?.Document?.Body;

            if (body is null)
            {
                throw new InvalidDataException(
                    "The DOCX file does not contain a readable document body.");
            }

            return body.InnerText;
        }
    }
}
