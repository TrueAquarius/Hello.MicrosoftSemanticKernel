using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.MicrosoftSemanticKernel.AzureAISearch
{
    internal static class GlobalConst
    {
        internal const string BlobStorageContainer = "ragmemory";
        internal const string AISearchVectorStoreServiceID = "ragvector";
        internal const int NumberOfChunksToFetch = 5;
        internal const int ChatHistoryLength = 10;
        internal const int ChnunkSize = 500;
        internal const int ChunkOverlap = 50;
    }
}
