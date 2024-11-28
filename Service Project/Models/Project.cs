namespace Service_Project.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string BlobContainerName { get; set; } // Name of the container in Blob Storage
}
