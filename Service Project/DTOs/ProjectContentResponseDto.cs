namespace Service_Project.DTOs;

public class ProjectContentResponseDto
{
    public Guid Id { get; set; }
    
    public Guid OwnerId { get; set; }  // Refers to the user who owns the project
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    public object Dictionary { get; set; }  // The file structure from the blob storage
}