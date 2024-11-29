namespace Service_Project.Models;

using Microsoft.EntityFrameworkCore;
public class BrewingStandDbContext : DbContext
{
    public BrewingStandDbContext(DbContextOptions<BrewingStandDbContext> options) : base(options)
    { }

    public DbSet<Project> Projects { get; set; }  // This represents the "projects" table
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Map the Project model to the 'projects' table
        modelBuilder.Entity<Project>().ToTable("projects", "projects");
    }
}