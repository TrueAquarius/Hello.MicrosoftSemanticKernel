# Demo.MicrosoftSemanticKernel.VectorDistance

An interesting experiment which compares distances of related phrases.

It uploads a list of phrases to a vertor DB. 

 ```csharp
 List<string> sentences = [
    "He is a carpenter",
    "He is a doctor",
    "He is a policeman",
    "He is a dentist",
    "He is a teacher",
    "He is a lawyer",
    ... many more ...
];
 ```

Thereafter is compares their embeddings with one other phrase:

 ```csharp
var searchString = "He is a software programmer";
var searchVector = await textEmbeddingGenerationService.GenerateEmbeddingAsync(searchString);


await foreach (var result in collection.SearchEmbeddingAsync(searchVector, sentences.Count))
{
    Console.WriteLine($"{result.Score:F5}   {result.Record.Profession}");
}
 ```

It outputs all phrases sorted by their disctance to the `serachString`:
 ```
0,98921   He is a software developer
0,98465   He is a programmer
0,92194   He is a systems analyst
0,92136   He is a web designer
0,91969   He is an IT consultant
0,91927   He is a game developer
0,91441   He is an engineer
0,90649   He is a project manager
  ... many more ...
 ```
One can see that phrases which are related to the search string are indeed on top on the list.

## Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or later
- C# 12.0
- Azure OpenAI Service 

## Configuration
- Set the following environment variables:
  - `AZURE_OPENAI_ENDPOINT`: The endpoint for the Azure OpenAI service.
  - `AZURE_OPENAI_API_KEY`: The API key for the Azure OpenAI service.
  - `AZURE_OPENAI_DEPLOYMENT_NAME`: The deployment name for the Azure OpenAI service.
  - `AZURE_OPENAI_EMBEDDING`: The deployment name for the embedding service (see notes).

## Links

Here are a couple of links which 'inspired' this demo:
- [Github > Microsoft > Semantic Kernel > DotNet > Notebooks > 06-vector-stores-and-embeddings.ipynb](https://github.com/microsoft/semantic-kernel/blob/main/dotnet/notebooks/06-vector-stores-and-embeddings.ipynb) (The information given in that notebook was already partly outdated when this demo was created.)


## Notes

- At the time when this demo was created, the Semantic Kernel framework seemed to still undergo a lot of changes. Therefore these code examples may not work in future releases of that framework.
- Microsoft Semantic Kernel version 1.49.0 was used for this demo.
- If you change the embedding model, you may need to change the vector length in the Glossary class accordingly.
- This demo uses pre-release packages of the Semantic Kernel framework: 
	- Microsoft.Extensions.VectorData.Abstractions (9.0.0-preview)

## License

This project is licensed under the [MIT License](../LICENSE.txt).

   
   