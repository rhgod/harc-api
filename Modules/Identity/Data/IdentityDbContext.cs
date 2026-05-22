using Microsoft.EntityFrameworkCore;

namespace Harc.Api.Modules.Identity.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bu DbContext içindeki tüm tablolar veritabanında "identity" şeması altına kurulur
        modelBuilder.HasDefaultSchema("identity");
        
        // Unique email için index
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}