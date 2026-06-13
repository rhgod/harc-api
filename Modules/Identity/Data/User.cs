using System.ComponentModel.DataAnnotations;

namespace Harc.Api.Modules.Identity.Data;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

