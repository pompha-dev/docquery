using DocumentFormat.OpenXml.Spreadsheet;

namespace RagApp.Api.Services
{
    public class CosineSimilarityService
    {
        public double Calculate(ReadOnlyMemory<float> vectorA, ReadOnlyMemory<float> vectorB)
        {
            var a = vectorA.Span;
            var b = vectorB.Span;

            if(a.Length != b.Length)
            {
                throw new ArgumentException("Vectors must have the same length", nameof(vectorB));
            }

            double dotProduct = 0;
            for(int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
            }

            double magnitudeA = 0;
            foreach(var i in a)
            {
                magnitudeA += i * i;
            }
            magnitudeA = Math.Sqrt(magnitudeA);

            double magnitudeB = 0;
            foreach(var i in b)
            {
                 magnitudeB += i * i;
            }
            magnitudeB = Math.Sqrt(magnitudeB);

            if(magnitudeA == 0 || magnitudeB == 0)
            {
                throw new ArgumentException("Cannot calculate similarity for a zero-magnitude vector.");
            }

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
