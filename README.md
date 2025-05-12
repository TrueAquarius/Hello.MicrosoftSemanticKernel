# Demo.MicrosoftSemanticKernel  

This repository contains a collection of projects demonstrating the use of the Microsoft Semantic Kernel.  
Each project implements specific features integrated with the Semantic Kernel.  

## Projects  

- **Demo.MicrosoftSemanticKernel.LightPlugin**: A plugin for controlling lights, offering features such as turning lights on/off, changing colors, and retrieving their color.  
- **Demo.MicrosoftSemanticKernel.LightPluginWithProxy**: same functionality as "Demo.MicrosoftSemanticKernel.LightPlugin". However, it allows accessing OpenAI through a proxy.
- **Demo.MicrosoftSemanticKernel.FunctionFromFile**: This project demonstrates how to use Semantic Functions from a file. It is a simple language translator.
- **Demo.MicrosoftSemanticKernel.VectorStores**: This project demonstrates how to create embeddings, use in-memory vector stores, perform searches in vector stores. 
- **Demo.MicrosoftSemanticKernel.VectorDistance**: An interesting experiment which compares distances of related texts. 
- **Demo.MicrosoftSemanticKernel.AzureAISearch**: A small RAG System using Microsoft Semantic Kernel and Azure OpenAI Service. 

## Prerequisites  

- .NET 8 SDK  
- Visual Studio 2022 or later  
- C# 12.0
- Azure OpenAI Service
- other sub-project specific (see project specific ReadMe files)

## Installation  

1. Clone this repository: git clone https://github.com/TrueAquarius/Demo.MicrosoftSemanticKernel.git
2. Open the solution in Visual Studio 2022.

## Notes

- At the time when this demo was created, the Semantic Kernel framework seemed to still undergo a lot of changes. Therefore these code examples may not work in future releases of that framework.
- Microsoft Semantic Kernel version 1.49.0 was used for these demos.


## License

This project is licensed under the [MIT License](LICENSE).

