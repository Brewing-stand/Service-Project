using Azure.Storage.Blobs;
using FluentResults;

namespace Service_Project.Repositories;

public class BlobRepository : IBlobRepository
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobRepository(string connectionString)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<Result> CreateContainerAsync(string containerName)
    {
        try
        {
            // Check if the container already exists
            if (await DoesContainerExistAsync(containerName))
                return Result.Fail(new Error($"Container with name '{containerName}' already exists."));

            // Create the container if it doesn't exist
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            // Set up the container structure
            var structureResult = await SetupBrewFolderStructureAsync(containerName);
            if (structureResult.IsFailed) return Result.Fail(structureResult.Errors);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error creating container: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteContainerAsync(string containerName)
    {
        throw new NotImplementedException();
    }

    private async Task<Result> SetupBrewFolderStructureAsync(string containerName)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            // Upload the HEAD file to point to refs/heads/main
            var headBlobClient = containerClient.GetBlobClient("HEAD");
            await headBlobClient.UploadAsync(new BinaryData("refs/heads/main"), true);

            // Upload the README.md file at the root of the container
            var readmeBlobClient = containerClient.GetBlobClient("README.md");
            await readmeBlobClient.UploadAsync(new BinaryData("Current README content from the latest commit"), true);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error setting up Brew folder structure: {ex.Message}"));
        }
    }

    private async Task<bool> DoesContainerExistAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        return await containerClient.ExistsAsync();
    }
}