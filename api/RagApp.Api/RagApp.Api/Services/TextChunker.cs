namespace RagApp.Api.Services
{
    public class TextChunker
    {
        public IReadOnlyList<string> Split(
            string text,
            int chunkSize = 1000,
            int overlap = 150)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(
                    "Document text cannot be empty.",
                    nameof(text));
            }

            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(chunkSize));
            }

            if (overlap < 0 || overlap >= chunkSize)
            {
                throw new ArgumentOutOfRangeException(nameof(overlap));
            }

            List<string> chunks = new List<string>();
            int start = 0;

            while (start < text.Length)
            {
                int remaining = text.Length - start;
                int length = Math.Min(remaining, chunkSize);

                string chunk = text.Substring(start, length);
                chunks.Add(chunk);

                if (remaining == length)
                {
                    break;
                }

                start = start + length - overlap;
            }

            return chunks;
        }
    }
}
