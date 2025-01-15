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
    public async Task<Result<List<Project>>> GetAllProjectsByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            // Return a failure result if the userId is an empty Guid
            return Result.Fail(new Error("User ID cannot be an empty GUID."));
        }

        try
        {
            // Query the projects by userId
            var projects = await _context.Projects
                .Where(p => p.ownerId == userId) // Assuming your Project entity has a UserId field
                .ToListAsync();

            if (projects == null || projects.Count == 0)
            {
                // Return a failure result if no projects are found
                return Result.Fail(new Error($"No projects found for user with ID {userId}."));
            }

            // Return success with the list of projects
            return Result.Ok(projects);
        }
        catch (Exception ex)
        {
            // Return a failure result with the exception message
            return Result.Fail(new Error($"An error occurred while retrieving projects: {ex.Message}"));
        }
    }

    public async Task<Result<Project>> GetProjectByIdAsync(Guid id, Guid userId)
    {
        try
        {
            // Attempt to find the project by ID and userId
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.id == id && p.ownerId == userId); // Ensure the project belongs to the user

            if (project == null)
            {
                // Return a failure result if the project is not found
                return Result.Fail(new Error($"Project with ID {id} not found"));
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

    public async Task<Result> AddProjectAsync(Project project, Guid userId)
    {
        if (project == null)
        {
            // Return a failed result if project is null
            return Result.Fail("Project cannot be null.");
        }

        try
        {
            // Assign the userId to the project to establish ownership
            project.ownerId = userId;

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

    public async Task<Result> UpdateProjectAsync(Project project, Guid userId)
    {
        try
        {
            // Ensure the project belongs to the user
            var existingProject = await _context.Projects
                .FirstOrDefaultAsync(p => p.id == project.id && p.ownerId == userId);

            if (existingProject == null)
            {
                return Result.Fail(new Error($"Project with id '{project.id}' not found for the user."));
            }

            // Update the project properties with the new values
            existingProject.name = project.name;
            existingProject.description = project.description;
            // Add other fields that you want to update

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error updating project: {ex.Message}"));
        }
    }
    
    public async Task<Result> DeleteProjectAsync(Guid id, Guid userId)
    {
        try
        {
            // Ensure the project belongs to the user
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.id == id && p.ownerId == userId);

            if (project == null)
            {
                return Result.Fail(new Error($"Project with id '{id}' not found for the user."));
            }

            // Remove the project from the database
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new Error($"Error deleting project: {ex.Message}"));
        }
    }
}