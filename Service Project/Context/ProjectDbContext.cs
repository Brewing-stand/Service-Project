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

    public virtual DbSet<Project> Projects { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("citus", "distribution_type", new[] { "hash", "range", "append" })
            .HasPostgresEnum("citus", "shard_transfer_mode", new[] { "auto", "force_logical", "block_writes" })
            .HasPostgresEnum("pg_catalog", "citus_copy_format", new[] { "csv", "binary", "text" })
            .HasPostgresEnum("pg_catalog", "citus_job_status", new[] { "scheduled", "running", "finished", "cancelling", "cancelled", "failing", "failed" })
            .HasPostgresEnum("pg_catalog", "citus_task_status", new[] { "blocked", "runnable", "running", "done", "cancelling", "error", "unscheduled", "cancelled" })
            .HasPostgresEnum("pg_catalog", "noderole", new[] { "primary", "secondary", "unavailable" })
            .HasPostgresExtension("partman", "pg_partman")
            .HasPostgresExtension("pg_catalog", "citus")
            .HasPostgresExtension("pg_catalog", "citus_columnar")
            .HasPostgresExtension("pg_catalog", "pg_cron")
            .HasPostgresExtension("pg_catalog", "pgaadauth")
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("citext")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("fuzzystrmatch")
            .HasPostgresExtension("hll")
            .HasPostgresExtension("hstore")
            .HasPostgresExtension("intarray")
            .HasPostgresExtension("ltree")
            .HasPostgresExtension("pg_buffercache")
            .HasPostgresExtension("pg_freespacemap")
            .HasPostgresExtension("pg_prewarm")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("pgrowlocks")
            .HasPostgresExtension("pgstattuple")
            .HasPostgresExtension("semver")
            .HasPostgresExtension("sslinfo")
            .HasPostgresExtension("tablefunc")
            .HasPostgresExtension("tdigest")
            .HasPostgresExtension("topn")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp")
            .HasPostgresExtension("xml2");

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.id).HasName("projects_pkey");

            entity.ToTable("projects", "projects");

            entity.Property(e => e.id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.description).HasColumnName("description ");
            entity.Property(e => e.name).HasColumnName("name");
        });
        
        modelBuilder.HasSequence("jobid_seq", "cron");
        modelBuilder.HasSequence("pg_dist_cleanup_recordid_seq", "pg_catalog");
        modelBuilder.HasSequence("pg_dist_colocationid_seq", "pg_catalog").HasMax(4294967296L);
        modelBuilder.HasSequence("pg_dist_groupid_seq", "pg_catalog").HasMax(4294967296L);
        modelBuilder.HasSequence("pg_dist_node_nodeid_seq", "pg_catalog").HasMax(4294967294L);
        modelBuilder.HasSequence("pg_dist_operationid_seq", "pg_catalog");
        modelBuilder.HasSequence("pg_dist_placement_placementid_seq", "pg_catalog");
        modelBuilder.HasSequence("pg_dist_shardid_seq", "pg_catalog").HasMin(102008L);
        modelBuilder.HasSequence("runid_seq", "cron");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
