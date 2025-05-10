using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public sealed class Glossary
    {
        [VectorStoreRecordKey]
        public ulong Key { get; set; }

        [VectorStoreRecordData]
        public string Profession { get; set; }

        [VectorStoreRecordVector(Dimensions: 1536)]
        public ReadOnlyMemory<float> DefinitionEmbedding { get; set; }
    }
