namespace Service_Project.Models;

public class Project
{
    public Guid id { get; set; }
    
    public Guid ownerId { get; set; }  // Refers to the user who owns the project
    public string name { get; set; }
    public string description { get; set; } // The file structure from the blob storage
}
