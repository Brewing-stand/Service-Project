using FluentResults;
using Microsoft.EntityFrameworkCore;
using Service_Project.Context;
using Service_Project.Models;

namespace Service_Project.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectDbContext _context;

    public ProjectRepository(ProjectDbContext context)
    {
        _context = context;
    }
    public Task<IEnumerable<Project>> GetAllProjectsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<Project>> GetProjectByIdAsync(Guid id)
    {
        try
        {
            // Attempt to find the project by ID
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.id == id);

            if (project == null)
            {
                // Return a failure result if the project is not found
                return Result.Fail(new Error($"Project with ID {id} not found."));
            }

            // Return success with the project
            return Result.Ok(project);
        }
        catch (Exception ex)
        {
            // Return a failure result with the exception message
            return Result.Fail(new Error($"An error occurred while retrieving the project: {ex.Message}"));
        }
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

    public Task DeleteProjectAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}