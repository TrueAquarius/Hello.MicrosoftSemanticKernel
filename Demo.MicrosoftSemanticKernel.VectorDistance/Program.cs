using Microsoft.SemanticKernel;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Embeddings;


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


List<string> sentences = [
    "He is a carpenter",
    "He is a doctor",
    "He is a policeman",
    "He is a dentist",
    "He is a teacher",
    "He is a lawyer",
    "He is a firefighter",
    "He is a chef",
    "He is an engineer",
    "He is a pilot",
    "He is a mechanic",
    "He is a pharmacist",
    "He is a nurse",
    "He is a scientist",
    "He is a soldier",
    "He is a journalist",
    "He is a librarian",
    "He is a musician",
    "He is an artist",
    "He is an actor",
    "He is a farmer",
    "He is a judge",
    "He is a barber",
    "He is a tailor",
    "He is a photographer",
    "He is a waiter",
    "He is a butcher",
    "He is a baker",
    "He is a plumber",
    "He is an electrician",
    "He is a vet",
    "He is a painter",
    "He is a translator",
    "He is a psychologist",
    "He is a counselor",
    "He is a geologist",
    "He is a biologist",
    "He is a physicist",
    "He is a mathematician",
    "He is a chemist",
    "He is a zoologist",
    "He is a botanist",
    "He is an economist",
    "He is an accountant",
    "He is a broker",
    "He is a banker",
    "He is a real estate agent",
    "He is a flight attendant",
    "He is a security guard",
    "He is a delivery driver",
    "He is a taxi driver",
    "He is a bus driver",
    "He is a truck driver",
    "He is a construction worker",
    "He is a welder",
    "He is a bricklayer",
    "He is a roofer",
    "He is a janitor",
    "He is a cleaner",
    "He is a mover",
    "He is a tour guide",
    "He is a receptionist",
    "He is a secretary",
    "He is a paralegal",
    "He is a programmer",
    "He is a software developer",
    "He is a web designer",
    "He is a game developer",
    "He is a systems analyst",
    "He is a network administrator",
    "He is an IT consultant",
    "He is a UX designer",
    "He is a data analyst",
    "He is a data scientist",
    "He is a marketing manager",
    "He is a sales manager",
    "He is a project manager",
    "He is a business analyst",
    "He is a HR manager",
    "He is a recruiter",
    "He is a logistics coordinator",
    "He is a supply chain manager",
    "He is a customer service representative",
    "He is a social media manager",
    "He is a public relations officer",
    "He is a content writer",
    "He is an editor",
    "He is a publisher",
    "He is a filmmaker",
    "He is a director",
    "He is a choreographer",
    "He is a dancer",
    "He is a model",
    "He is a fashion designer",
    "He is an interior designer",
    "He is an architect",
    "He is a surveyor",
    "He is a civil engineer",
    "He is a mechanical engineer",
    "He is an aerospace engineer",
    "He is a naval officer",
    "He is a customs officer",
    "He is an immigration officer",
    "He is a missionary"
];


#pragma warning disable SKEXP0020

// Define vector store
var vectorStore = new InMemoryVectorStore();

// Get a collection instance using vector store
var collection = vectorStore.GetCollection<ulong, Glossary>("skglossary");

// Get a collection instance by initializing it directly
//var collection2 = new InMemoryVectorStoreRecordCollection<ulong, Glossary>("skglossary");

await collection.CreateCollectionIfNotExistsAsync();

var glossaryEntries = new List<Glossary>();

foreach (var sentence in sentences)
{
    glossaryEntries.Add(new Glossary()
    {
        Key = (ulong)glossaryEntries.Count + 1,
        Profession = sentence,
    });
}



#pragma warning disable SKEXP0001

var textEmbeddingGenerationService = kernel.GetRequiredService<ITextEmbeddingGenerationService>();

var tasks = glossaryEntries.Select(entry => Task.Run(async () =>
{
    entry.DefinitionEmbedding = await textEmbeddingGenerationService.GenerateEmbeddingAsync(entry.Profession);
}));

await Task.WhenAll(tasks);

foreach(var entry in glossaryEntries)
{
    collection.UpsertAsync(entry).Wait();
    // Console.WriteLine($"Added '{entry.Profession}' to the collection.");
}

var options = new GetRecordOptions() { IncludeVectors = true };




#pragma warning disable SKEXP0001

var searchString = "He is a software programmer";
var searchVector = await textEmbeddingGenerationService.GenerateEmbeddingAsync(searchString);


await foreach (var result in collection.SearchEmbeddingAsync(searchVector, sentences.Count))
{
    Console.WriteLine($"{result.Score:F5}   {result.Record.Profession}");
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
