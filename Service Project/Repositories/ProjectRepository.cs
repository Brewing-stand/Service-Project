using Service_Project.Models;

namespace Service_Project.Repositories;

public class ProjectRepository : IProjectRepository
{
    public Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Project> GetProjectByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task UpdateProjectAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProjectAsync(int id)
    {
        throw new NotImplementedException();
    }
}