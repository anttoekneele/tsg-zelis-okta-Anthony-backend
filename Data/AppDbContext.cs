using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // Database Tables
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Claim> Claims { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<SecurityEvent> SecurityEvents { get; set; }

    // Dependency Injection Constructor
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Method to configure how C# classes map to database schema.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.ExternalId)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(320);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict); // A Role must always exist
        });

        // Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(r => r.Name)
                .IsUnique();

            entity.Property(r => r.Description)
                .HasMaxLength(200);

            entity.HasMany(r => r.RoleClaims)
                .WithOne(rc => rc.Role)
                .HasForeignKey(rc => rc.RoleId);
        });

        // Claim
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(c => c.Value)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(c => c.RoleClaims)
                .WithOne(rc => rc.Claim)
                .HasForeignKey(rc => rc.ClaimId);
        });

        // RoleClaim (join table for many to many Role <-> Claim)
        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasKey(rc => new { rc.RoleId, rc.ClaimId });

            entity.HasOne(rc => rc.Role)
                .WithMany(r => r.RoleClaims)
                .HasForeignKey(rc => rc.RoleId);

            entity.HasOne(rc => rc.Claim)
                .WithMany()
                .HasForeignKey(rc => rc.ClaimId);
        });

        // SecurityEvent
        modelBuilder.Entity<SecurityEvent>(entity =>
        {
            entity.HasKey(se => se.Id);

            entity.Property(se => se.EventType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(se => se.OccuredUtc)
                .IsRequired()
                .HasDefaultValueSql("SYSUTCDATETIME()");

            entity.Property(se => se.Details)
                .HasMaxLength(400);

            entity.HasOne(se => se.AuthorUser)
                .WithMany()
                .HasForeignKey(se => se.AuthorUserId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete

            entity.HasOne(se => se.AffectedUser)
                .WithMany()
                .HasForeignKey(se => se.AffectedUserId)
                .OnDelete(DeleteBehavior.Restrict); // prevent cascade delete
        });
    }
}