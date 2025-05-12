using Microsoft.Extensions.VectorData;

namespace Demo.MicrosoftSemanticKernel.AzureAISearch
{
    public class VectorStoreRecord
    {
        [VectorStoreRecordKey]
        public string Id { get; set; }

        [VectorStoreRecordData]
        public string Description { get; set; }

        [VectorStoreRecordData]
        public string Text { get; set; }

        [VectorStoreRecordData]
        public bool IsReference { get; set; }

        [VectorStoreRecordData]
        public string ExternalSourceName { get; set; }

        [VectorStoreRecordData]
        public string AdditionalMetadata { get; set; }

        [VectorStoreRecordData]
        public string Hello { get; set; }

        [VectorStoreRecordVector(1536)] 
        public ReadOnlyMemory<float> Embedding { get; set; }
    }
}
