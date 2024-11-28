namespace Service_Project.DTOs;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string BlobContainerName { get; set; }
}