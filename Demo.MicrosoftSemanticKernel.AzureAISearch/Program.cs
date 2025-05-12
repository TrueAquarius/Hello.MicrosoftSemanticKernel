using Microsoft.SemanticKernel;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Azure;
using Demo.MicrosoftSemanticKernel.AzureAISearch;
using Microsoft.SemanticKernel.ChatCompletion;


// Some of the NuGet Packages are still in pre-release or alpha state. Need to suppress compiler warnings
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0010


// Replace with your own OpenAI API key and model  
string azureOpenAIAPIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string azureOpenAIDeploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o";
string azureOpenAIEmbeddingModelName = Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING") ?? "text-embedding-ada-002";
string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureAISearchEndpoint = Environment.GetEnvironmentVariable("AZURE_AISEARCH_ENDPOINT");
string azureAISearchAPIKey = Environment.GetEnvironmentVariable("AZURE_AISEARCH_API_KEY");

if (string.IsNullOrEmpty(azureOpenAIAPIKey)
    || string.IsNullOrEmpty(azureOpenAIDeploymentName)
    || string.IsNullOrEmpty(azureOpenAIEmbeddingModelName)
    || string.IsNullOrEmpty(azureOpenAIEndpoint) 
    || string.IsNullOrEmpty(azureAISearchEndpoint) 
    || string.IsNullOrEmpty(azureAISearchAPIKey))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Please set relevant environment variables.");
    Console.ResetColor();
    return;
}


// Configure and build the kernel
Console.WriteLine($"Configuring and building the kernel");
var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAITextEmbeddingGeneration(azureOpenAIEmbeddingModelName, azureOpenAIEndpoint, azureOpenAIAPIKey);
builder.AddAzureOpenAIChatCompletion(azureOpenAIDeploymentName, azureOpenAIEndpoint, azureOpenAIAPIKey);
builder.AddAzureAISearchVectorStore(
        new Uri(azureAISearchEndpoint),
        new AzureKeyCredential(azureAISearchAPIKey), 
        null, 
        GlobalConst.AISearchVectorStoreServiceID);
var kernel = builder.Build();


// Set path where to find the test data files to foll the RAG Storage
string path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Test Data");

if (!Directory.Exists(path))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"The directory '{path}' does not exist.");
    Console.ResetColor();
    return;
}


// Set up vector store and embedding generator
Console.WriteLine($"Set up vector store and embedding generator");
var embeddingGenerator = kernel.GetRequiredService<ITextEmbeddingGenerationService>();
var vectorStore = kernel.GetRequiredService<IVectorStore>() as AzureAISearchVectorStore;
var collection = vectorStore.GetCollection<string, VectorStoreRecord>(GlobalConst.AISearchVectorStoreServiceID);
await collection.CreateCollectionIfNotExistsAsync();

// Get all files in the directory and subdirectories  
var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

// Chunk files and create embeddings
int cnt = 0;
foreach (var filePath in files)
{
    List<string> chunks = Chunker.ChunkText(File.ReadAllText(filePath), GlobalConst.ChnunkSize, GlobalConst.ChunkOverlap);

    foreach (var chunk in chunks)
    {
        ++cnt;
        Console.WriteLine($"Processing chunk {cnt} of {filePath}");
        var embedding = await embeddingGenerator.GenerateEmbeddingAsync(chunk);

        var record = new VectorStoreRecord
        {
            Id = Guid.NewGuid().ToString(),
            Description = "Document chunk",
            Text = chunk,
            AdditionalMetadata = $"source={filePath}",
            Embedding = embedding
        };

        await collection.UpsertAsync(record);
    }
}



// Start the Chat with the user
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n\n=== start Chatting with your documents ===");
Console.WriteLine("Type 'exit' or 'quit' to break.\n\n");
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings
{
    MaxTokens = 1000,
};

var history = new ChatHistory();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("> ");
    string input = Console.ReadLine();

    if (input?.Trim().ToLower() is "exit" or "quit") break;

    // Embed the user input
    var inputEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(input);

    string retrievedText = "";
    await foreach (var r in collection.SearchEmbeddingAsync(inputEmbedding, GlobalConst.NumberOfChunksToFetch))
    {
        retrievedText += $"{r.Record.Text}\n";
    }

    // Combine retrieved content into a system message or context
    string context = string.Join("\n\n", retrievedText);
    string promptWithContext = $"Use the following context to answer the question:\n{context}\n\nQuestion: {input}";

    history.AddUserMessage(promptWithContext);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        kernel: kernel
    );

    history.AddMessage(result.Role, result.Content ?? "");
    while(history.Count > GlobalConst.ChatHistoryLength)
    {
        history.RemoveAt(0);
    }

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\nAI Agent:\n{result.Content}\n");
}


// Clean up and say bye  
await collection.DeleteCollectionAsync();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\nBye bye!\nIt was fun to talk to you.\n\n\n");
Console.ResetColor();
