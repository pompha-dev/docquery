using OpenAI;
using OpenAI.Chat;
using RagApp.Api.Models;

namespace RagApp.Api.Services
{
    public class ChatService
    {
        private readonly ChatClient _chatClient;
        public ChatService(OpenAIClient openAIClient)
        {
            _chatClient = openAIClient.GetChatClient("gpt-4o-mini");
        }

        public async Task<string> GenerateAnswerAsync(string question, List<DocumentChunk> relevantChunks)
        {
            var context = string.Join("\n\n", relevantChunks.Select(chunk => chunk.Text));
            var messages = new List<ChatMessage>
            {
                 new SystemChatMessage("You are a helpful document assistant. Answer the user's question using only the provided document context. If the answer is not in the context, say you don't know."),
                 new UserChatMessage($"Document context:\n{context}\n\nQuestion:\n{question}")
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            var answer = response.Value.Content[0].Text;

            return answer;

        }
    }
}
