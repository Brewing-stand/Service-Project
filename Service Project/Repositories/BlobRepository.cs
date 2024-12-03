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

    public async Task<Result> CreateContainerAsync(Guid id)
    {
        try
        {
            // Check if the container already exists
            if (await DoesContainerExistAsync(id))
                return Result.Fail(new Error($"Container with name '{id}' already exists."));

            // Create the container if it doesn't exist
            var containerClient = _blobServiceClient.GetBlobContainerClient(id.ToString());
            await containerClient.CreateIfNotExistsAsync();

            // Set up the container structure
            var structureResult = await SetupBrewFolderStructureAsync(id);
            if (structureResult.IsFailed) return Result.Fail(structureResult.Errors);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error creating container: {ex.Message}"));
        }
    }

    public async Task<Result> DeleteContainerAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    private async Task<Result> SetupBrewFolderStructureAsync(Guid id)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(id.ToString());

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

    private async Task<bool> DoesContainerExistAsync(Guid id)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(id.ToString());
        return await containerClient.ExistsAsync();
    }
}