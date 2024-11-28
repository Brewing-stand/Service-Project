using FluentResults;

namespace Service_Project.Repositories;

public interface IBlobRepository
{
    Task<Result> CreateContainerAsync(string containerName);
    Task<Result> DeleteContainerAsync(string containerName);
}