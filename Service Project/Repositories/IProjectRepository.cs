using FluentResults;
using Service_Project.Models;

namespace Service_Project.Repositories;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<Project> GetProjectByIdAsync(int id);
    Task<Result> AddProjectAsync(Project project);
    Task UpdateProjectAsync(Project project);
    Task DeleteProjectAsync(int id);
}