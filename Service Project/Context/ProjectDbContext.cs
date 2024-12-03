using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Service_Project.Models;

namespace Service_Project.Context;


public partial class ProjectDbContext : DbContext
{
    public ProjectDbContext()
    {
    }

    public ProjectDbContext(DbContextOptions<ProjectDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Only keep the necessary configurations

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => e.id).HasName("projects_pkey");

                entity.ToTable("projects", "projects");

                entity.Property(e => e.id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");
                
                entity.Property(e => e.ownerId)
                    .HasColumnName("owner_id")
                    .IsRequired();
                
                entity.Property(e => e.description)
                    .HasColumnName("description");

                entity.Property(e => e.name)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    
}
