using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bu DbContext içindeki tüm tablolar veritabanında "identity" şeması altına kurulur
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.HasIndex(r => r.Name).IsUnique();

            entity.Property(r => r.DisplayName)
                  .HasColumnType("jsonb")
                  .IsRequired();
        });
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.RoleId).IsRequired();
        });
    }
}