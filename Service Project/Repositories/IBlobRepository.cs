using FluentResults;

namespace Service_Project.Repositories;

public interface IBlobRepository
{
    Task<Result> CreateContainerAsync(Guid id);
    Task<Result> DeleteContainerAsync(Guid id);
}