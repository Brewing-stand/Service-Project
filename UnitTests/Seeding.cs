using Service_Project.Context;
using Service_Project.Models;

namespace UnitTests;

public static class Seeding
{
    public static void InitializeTestDB(ProjectDbContext db)
    {
        db.Projects.AddRange(GetProjects());
        db.SaveChanges();
    }
    
    private static List<Project> GetProjects()
    {
        // Create sample user and project data
        var userId = new Guid("11111111-1111-1111-1111-111111111110"); // Simulating a user GUID

        return new List<Project>()
        {
            new Project
            {
                id = new Guid("11111111-1111-1111-1111-111111111111"),
                ownerId = userId,
                name = "Project One",
                description = "This is the description for Project One."
            },
            new Project
            {
                id = new Guid("11111111-1111-1111-1111-111111111112"),
                ownerId = userId,
                name = "Project Two",
                description = "This is the description for Project Two."
            },
            new Project
            {
                id = new Guid("11111111-1111-1111-1111-111111111113"),
                ownerId = userId,
                name = "Project Three",
                description = "This is the description for Project Three."
            }
        };
    }
}