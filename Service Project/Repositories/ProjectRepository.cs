using FluentResults;
using Microsoft.EntityFrameworkCore;
using Service_Project.Models;

namespace Service_Project.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly BrewingStandDbContext _context;

    public ProjectRepository(BrewingStandDbContext context)
    {
        _context = context;
    }
    public Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Project> GetProjectByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> AddProjectAsync(Project project)
    {
        if (project == null)
        {
            // Return a failed result if project is null
            return Result.Fail("Project cannot be null.");
        }

        try
        {
            // Add the new project to the DbSet
            await _context.Projects.AddAsync(project);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success result with the newly created project ID
            return Result.Ok().WithSuccess($"Project {project.name} created successfully with ID {project.id}");
        }
        catch (DbUpdateException dbEx)
        {
            // Log the exception or inspect the details
            Console.WriteLine(dbEx.Message);
            Console.WriteLine(dbEx.InnerException?.Message);  // This will give you more details
            
            // Return a detailed error message for database update errors
            return Result.Fail($"Database error: {dbEx.Message}");
        }
        catch (Exception ex)
        {
            // Return a general error message if something goes wrong
            return Result.Fail($"An unexpected error occurred while adding the project: {ex.Message}");
        }
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