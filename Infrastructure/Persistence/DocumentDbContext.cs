using DocumentManagement.Domain.Entities;
using DocumentManagement.Domain.Enums;
using DocumentManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DocumentManagement.Infrastructure.Persistence;

public class DocumentDbContext : DbContext
{
    public DocumentDbContext(DbContextOptions<DocumentDbContext> options) : base(options)
    {
    }

    public DbSet<Property> Properties => Set<Property>();
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<DocumentFile> Files => Set<DocumentFile>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<AuditEntry> AuditEntries => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>();
        modelBuilder.HasPostgresEnum<AuditAction>();

        ConfigureProperty(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureFolder(modelBuilder);
        ConfigureFile(modelBuilder);
        ConfigurePermission(modelBuilder);
        ConfigureAudit(modelBuilder);
    }

    private static void ConfigureProperty(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Property>();
        entity.ToTable("properties");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Id).HasColumnName("id");
        entity.Property(p => p.Name).HasColumnName("name").IsRequired().HasMaxLength(150);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<UserAccount>();
        entity.ToTable("users");
        entity.HasKey(u => u.Id);
        entity.Property(u => u.Id).HasColumnName("id");
        entity.Property(u => u.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(150);
        entity.Property(u => u.Email).HasColumnName("email").IsRequired();
        entity.HasIndex(u => u.Email).IsUnique();
        entity.Property(u => u.Role).HasColumnName("role").HasConversion<string>();
    }

    private static void ConfigureFolder(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Folder>();
        entity.ToTable("folders");
        entity.HasKey(f => f.Id);
        entity.Property(f => f.Id).HasColumnName("id");
        entity.Property(f => f.PropertyId).HasColumnName("property_id");
        entity.Property(f => f.ParentFolderId).HasColumnName("parent_folder_id");
        entity.Property(f => f.Name)
            .HasConversion(name => name.Value, value => new FolderName(value))
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
        entity.Property(f => f.Depth)
            .HasConversion(depth => depth.Value, value => new FolderDepth(value))
            .HasColumnName("depth")
            .IsRequired();

        entity.HasIndex(f => new { f.PropertyId, f.ParentFolderId, f.Name }).IsUnique();
        entity.HasOne<Folder>()
            .WithMany("_children")
            .HasForeignKey(f => f.ParentFolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureFile(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<DocumentFile>();
        entity.ToTable("files");
        entity.HasKey(f => f.Id);
        entity.Property(f => f.Id).HasColumnName("id");
        entity.Property(f => f.FolderId).HasColumnName("folder_id");
        entity.Property(f => f.Name)
            .HasConversion(name => name.Value, value => new FileName(value))
            .HasColumnName("name")
            .HasMaxLength(150)
            .IsRequired();
        entity.Property(f => f.Description)
            .HasConversion(description => description.Value, value => new DescriptionText(value))
            .HasColumnName("description")
            .HasMaxLength(500);
        entity.Property(f => f.Size)
            .HasConversion(size => size.Value, value => new FileSize(value))
            .HasColumnName("size_bytes")
            .IsRequired();
        entity.Property(f => f.CreatedAt).HasColumnName("created_at");

        entity.HasOne<Folder>()
            .WithMany("_files")
            .HasForeignKey(f => f.FolderId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigurePermission(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Permission>();
        entity.ToTable("permissions");
        entity.HasKey(p => p.Id);
        entity.Property(p => p.Id).HasColumnName("id");
        entity.Property(p => p.FolderId).HasColumnName("folder_id");
        entity.Property(p => p.UserId).HasColumnName("user_id");
        entity.Property(p => p.Access).HasColumnName("access").HasConversion<string>();
        entity.HasIndex(p => new { p.FolderId, p.UserId }).IsUnique();

        entity.HasOne<Folder>()
            .WithMany("_permissions")
            .HasForeignKey(p => p.FolderId);
        entity.HasOne<UserAccount>()
            .WithMany()
            .HasForeignKey(p => p.UserId);
    }

    private static void ConfigureAudit(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AuditEntry>();
        entity.ToTable("audit_entries");
        entity.HasKey(a => a.Id);
        entity.Property(a => a.Id).HasColumnName("id");
        entity.Property(a => a.PropertyId).HasColumnName("property_id");
        entity.Property(a => a.FolderId).HasColumnName("folder_id");
        entity.Property(a => a.FileId).HasColumnName("file_id");
        entity.Property(a => a.ActorId).HasColumnName("actor_id");
        entity.Property(a => a.Action).HasColumnName("action").HasConversion<string>();
        entity.Property(a => a.OccurredAt).HasColumnName("occurred_at");
        entity.Property(a => a.Details).HasColumnName("details").HasMaxLength(500);
    }
}
