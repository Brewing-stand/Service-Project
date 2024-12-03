using FluentResults;
using Service_Project.Models;

namespace Service_Project.Repositories;

public interface IProjectRepository
{
    Task<Result<List<Project>>> GetAllProjectsByUserIdAsync(string? userId);
    Task<Result<Project>> GetProjectByIdAsync(Guid id);
    Task<Result> AddProjectAsync(Project project);
    Task<Result> UpdateProjectAsync(Project project);
    Task<Result> DeleteProjectAsync(Guid id);
}