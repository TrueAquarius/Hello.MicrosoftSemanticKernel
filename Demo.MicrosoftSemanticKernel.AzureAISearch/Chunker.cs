using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.MicrosoftSemanticKernel.AzureAISearch
{
    public class Chunker
    {
        public static List<string> ChunkText(string text, int chunkSize = 500, int overlap = 0)
        {
            List<string> chunks = new List<string>();
            for (int i = 0; i < text.Length; i += chunkSize - overlap)
            {
                chunks.Add(text.Substring(i, Math.Min(chunkSize, text.Length - i)));
            }
            return chunks;
        }
    }
}
