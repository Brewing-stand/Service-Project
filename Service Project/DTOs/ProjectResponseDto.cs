namespace Service_Project.DTOs;

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    
    public Guid OwnerId { get; set; }  // Refers to the user who owns the project
    
    public string Name { get; set; }
    
    public string Description { get; set; }
}