using System.Text;
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

    public async Task<Result<Dictionary<string, List<string>>>> GetContainerContent(Guid id)
    {
        try
        {
            // Use the DoesContainerExistAsync method to check if the container exists
            if (!await DoesContainerExistAsync(id))
                return Result.Fail(new Error($"Container with id '{id}' does not exist."));

            // Get the container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(id.ToString());

            // Initialize a dictionary to represent the file structure
            var fileStructure = new Dictionary<string, List<string>>();

            // Retrieve a list of all blobs in the container
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                // Use '/' as a delimiter to mimic directory structure
                var paths = blobItem.Name.Split('/');

                // If it's the first level (root), create an entry for it
                if (paths.Length == 1)
                {
                    if (!fileStructure.ContainsKey("root"))
                    {
                        fileStructure["root"] = new List<string>();
                    }
                    fileStructure["root"].Add(paths[0]);
                }
                else
                {
                    // For deeper levels (subdirectories), add them accordingly
                    var parentDir = paths[0]; // First part is the parent directory
                    var fileName = paths[1]; // Second part is the file

                    if (!fileStructure.ContainsKey(parentDir))
                    {
                        fileStructure[parentDir] = new List<string>();
                    }
                    fileStructure[parentDir].Add(fileName);
                }
            }

            // Return the file structure as a result
            return Result.Ok(fileStructure);
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error retrieving content from container: {ex.Message}"));
        }
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