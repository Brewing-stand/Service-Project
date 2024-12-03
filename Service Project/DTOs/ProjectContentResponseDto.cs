namespace Service_Project.DTOs;

public class ProjectContentResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public object Dictionary { get; set; }  // The file structure from the blob storage
}