using System.Net;
using System.Net.Http;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Replace with your own OpenAI API key and model  
string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o";
string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");

ConsoleColor oldColor = Console.ForegroundColor;

if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(deployment) || string.IsNullOrEmpty(endpoint))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Please set the AZURE_OPENAI_API_KEY, AZURE_OPENAI_DEPLOYMENT, and AZURE_OPENAI_ENDPOINT environment variables.");
    Console.ForegroundColor = oldColor;
    return;
}

// Build and configure kernel  
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(
   deploymentName: deployment,
   endpoint: endpoint,
   apiKey: apiKey
);

var kernel = builder.Build();


// Plugin directory path
var myPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "..", "..", "..", "prompt_templates");

if(!Path.Exists(myPluginDirectoryPath))
{
    Console.ForegroundColor= ConsoleColor.Red;
    Console.WriteLine("Path does not exist.");
    Console.ForegroundColor = oldColor;
    return;
}

// Load the myPlugin from the Plugins Directory
var myPluginFunctions = kernel.ImportPluginFromPromptDirectory(myPluginDirectoryPath);

if (myPluginFunctions.Count() == 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("No plugins found.");
    Console.ForegroundColor = oldColor;
    return;
}



// Start the Chat with the user  
Console.WriteLine("=== Translate English to German ===");
Console.WriteLine("Type something, I will translate.\nType 'exit' or 'quit' to break.\n\n");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("> ");
    string input = Console.ReadLine();

    if (input?.Trim().ToLower() is "exit" or "quit") break;

    // Construct arguments
    var arguments = new KernelArguments() { ["input"] = input };

    // Run the Function called Joke
    var result = await kernel.InvokeAsync(myPluginFunctions["Translate"], arguments);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n{result}\n");
}

// Clean up and say bye  
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\nBye bye!\nIt was fun to talk to you.\n\n\n");
Console.ForegroundColor = oldColor;
