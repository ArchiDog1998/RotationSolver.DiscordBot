using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace RotationSolver.DiscordBot;
internal static class GptHelper
{
    public static AsyncResultCollection<StreamingChatCompletionUpdate> GptTalk(string inputString)
    {
        var option = new OpenAIClientOptions()
        {
            Endpoint = new Uri(Config.ChatGptUri)
        };

        var client = new OpenAIClient(new ApiKeyCredential(Config.ChatGptKey), option);

        var chat = client.GetChatClient("gpt-4o");

        return chat.CompleteChatStreamingAsync(new UserChatMessage(inputString));
    }
}
