using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Title> Titles => Set<Title>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.HasIndex(r => r.Name).IsUnique();
            entity.Property(r => r.DisplayName).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.DisplayName).HasColumnType("jsonb").IsRequired();
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.Property(t => t.Id).ValueGeneratedOnAdd();
            entity.HasIndex(t => t.Name).IsUnique();
            entity.Property(t => t.DisplayName).HasColumnType("jsonb").IsRequired();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Team)
                .WithMany()
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(u => u.Title)
                .WithMany(t => t.Users)
                .HasForeignKey(u => u.TitleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.Manager)
                .WithMany()
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}