using FluentResults;
using Service_Project.Models;

namespace Service_Project.Repositories;

public interface IProjectRepository
{
    Task<Result<List<Project>>> GetAllProjectsByUserIdAsync(Guid userId);
    Task<Result<Project>> GetProjectByIdAsync(Guid id, Guid userId);
    Task<Result> AddProjectAsync(Project project, Guid userId);
    Task<Result> UpdateProjectAsync(Project project, Guid userId);
    Task<Result> DeleteProjectAsync(Guid id, Guid userId);
}