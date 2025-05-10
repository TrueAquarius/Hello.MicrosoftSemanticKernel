using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;
using System.Text.Json;


// Replace with your own OpenAI API key and model  
string apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deployment = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT") ?? "gpt-4o";
string embedding = Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING") ?? "text-embedding-ada-002";
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
var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0010
builder.AddAzureOpenAITextEmbeddingGeneration(embedding, endpoint, apiKey); 

var kernel = builder.Build();


#pragma warning disable SKEXP0020

// Define vector store
var vectorStore = new InMemoryVectorStore();

// Get a collection instance using vector store
var collection = vectorStore.GetCollection<ulong, Glossary>("skglossary");

// Get a collection instance by initializing it directly
var collection2 = new InMemoryVectorStoreRecordCollection<ulong, Glossary>("skglossary");

await collection.CreateCollectionIfNotExistsAsync();

var glossaryEntries = new List<Glossary>()
{
    new Glossary()
    {
        Key = 1,
        Term = "API",
        Definition = "Application Programming Interface. A set of rules and specifications that allow software components to communicate and exchange data.",
        MyDummyRecord = "This is my first dummy record."
    },
    new Glossary()
    {
        Key = 2,
        Term = "Connectors",
        Definition = "Connectors allow you to integrate with various services provide AI capabilities, including LLM, AudioToText, TextToAudio, Embedding generation, etc.",
        MyDummyRecord = "This is my second dummy record."
    },
    new Glossary()
    {
        Key = 3,
        Term = "RAG",
        Definition = "Retrieval Augmented Generation - a term that refers to the process of retrieving additional data to provide as context to an LLM to use when generating a response (completion) to a user's question (prompt).",
        MyDummyRecord = "This is my third dummy record."
    }
};

#pragma warning disable SKEXP0001

var textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
{
    entry.DefinitionEmbedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(entry.Definition);
}));

await Task.WhenAll(tasks);

foreach(var entry in glossaryEntries)
{
    collection.UpsertAsync(entry).Wait();
    Console.WriteLine($"Added {entry.Term} to the collection.");
}

var options = new GetRecordOptions() { IncludeVectors = true };

for(ulong i=1; i<=3; ++i)
{
    var record = await collection.GetAsync(key: i, options);
    Console.WriteLine($"Key: {record.Key}");
    Console.WriteLine($"Term: {record.Term}");
    Console.WriteLine($"Definition: {record.Definition}");
    Console.WriteLine($"MyDummyRecord: {record.MyDummyRecord}");
    //Console.WriteLine($"Definition Embedding: {JsonSerializer.Serialize(record.DefinitionEmbedding)}");
}


#pragma warning disable SKEXP0001

var searchString = "I want to learn more about Connectors";
var searchVector = await textEmbeddingGenerationService.GenerateEmbeddingAsync(searchString);


await foreach (var result in collection.SearchEmbeddingAsync(searchVector, 5))
{
    Console.WriteLine($"Search score: {result.Score}");
    Console.WriteLine($"Key: {result.Record.Key}");
    Console.WriteLine($"Term: {result.Record.Term}");
    Console.WriteLine($"Definition: {result.Record.Definition}");
    Console.WriteLine($"MyDummyRecord: {result.Record.MyDummyRecord}");
    Console.WriteLine("=========");
}










#if false
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

    // Run the function called "Translate" (same as the sub-folder name under "prompt_templates")
    var result = await kernel.InvokeAsync(myPluginFunctions["Translate"], arguments);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"\n{result}\n");
}
#endif

// Clean up and say bye  
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\nBye bye!\nIt was fun to talk to you.\n\n\n");
Console.ForegroundColor = oldColor;
