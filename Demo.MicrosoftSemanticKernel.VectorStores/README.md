# Demo.MicrosoftSemanticKernel.VectorStores

This project demonstrates how to create embeddings, use in-memory vector stores, perform searches in vector stores.

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

   
   