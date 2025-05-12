using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace Demo.MicrosoftSemanticKernel.AzureAISearch
{
    internal class BlobStorageUtil
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageUtil(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;

            EnsureContainerExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>  
        /// Ensures that a container exists. If it does not exist, it will be created.  
        /// </summary>  
        /// <param name="containerName">The name of the container to ensure or create.</param>  
        public async Task EnsureContainerExistsAsync()
        {
            // Create a BlobServiceClient to interact with the Blob service  
            var blobServiceClient = new BlobServiceClient(_connectionString);

            // Get a reference to the container  
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure the container exists  
            await containerClient.CreateIfNotExistsAsync();
        }




        public async Task UploadFileAsync(string filePath, string blobName = null)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                blobName = Path.GetFileName(filePath);
            }

            // Create a BlobServiceClient to interact with the Blob service  
            var blobServiceClient = new BlobServiceClient(_connectionString);

            // Get a reference to the container  
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure the container exists  
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to the blob  
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload the file  
            //using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            await blobClient.UploadAsync(filePath, overwrite: true);
        }



        /// <summary>  
        /// Uploads all files from a specified directory to the blob storage container.  
        /// This method processes files recursively.  
        /// </summary>  
        /// <param name="directoryPath">The path of the directory containing files to upload.</param>  
        public async Task UploadAllFilesAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"The directory '{directoryPath}' does not exist.");
            }


            // Get all files in the directory and subdirectories  
            var files = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

            foreach (var filePath in files)
            {
                await UploadFileAsync(filePath);
            }
        }





        /// <summary>  
        /// Deletes all blobs from the specified container.  
        /// </summary>  
        /// <param name="containerName">The name of the container from which to delete all blobs.</param>  
        public async Task DeleteAllBlobsAsync()
        {
            // Create a BlobServiceClient to interact with the Blob service  
            var blobServiceClient = new BlobServiceClient(_connectionString);

            // Get a reference to the container  
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Ensure the container exists  
            if (await containerClient.ExistsAsync())
            {
                // List all blobs in the container  
                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    // Get a reference to each blob  
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);

                    // Delete the blob  
                    await blobClient.DeleteIfExistsAsync();
                }
            }
        }

    }
}
