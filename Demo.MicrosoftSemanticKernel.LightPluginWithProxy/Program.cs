using Demo.MicrosoftSemanticKernel.LightPluginWithProxy;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Net.Http;

// Replace with your own OpenAI API key and model  
string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o";
string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string proxy = Environment.GetEnvironmentVariable("AZURE_OPENAI_PROXY");

if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(deployment) || string.IsNullOrEmpty(endpoint))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Please set the AZURE_OPENAI_API_KEY, AZURE_OPENAI_DEPLOYMENT, AZURE_OPENAI_ENDPOINT, and AZURE_OPENAI_PROXY environment variables.");
    return;
}

// Configure HttpClient with proxy  
var httpClientHandler = new HttpClientHandler
{
    Proxy = new System.Net.WebProxy(proxy),
    UseProxy = string.IsNullOrEmpty(proxy)
};

var httpClient = new HttpClient(httpClientHandler)
{
    BaseAddress = new Uri(endpoint)
};

// Build and configure kernel  
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
   deploymentName: deployment,
   endpoint: endpoint,
   apiKey: apiKey,
   httpClient: httpClient
);

var kernel = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<LightsPlugin>("Lights");

OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    MaxTokens = 1000,
};

// Start the Chat with the user  
ConsoleColor oldColor = Console.ForegroundColor;
Console.WriteLine("=== Semantic Kernel Light Controller ===");
Console.WriteLine("Type something like:\n- 'Turn on the porch light'\n- 'What lights are on?'\nType 'exit' or 'quit' to quit.\n");

var history = new ChatHistory();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("> ");
    string input = Console.ReadLine();

    if (input?.Trim().ToLower() is "exit" or "quit") break;

    history.AddUserMessage(input);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        kernel: kernel
    );

    history.AddMessage(result.Role, result.Content ?? "");

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\nAI Agent:\n{result.Content}\n");
}

// Clean up and say bye  
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\nBye bye!\nIt was fun to talk to you.\n\n\n");
Console.ForegroundColor = oldColor;
