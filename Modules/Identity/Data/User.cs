using System.ComponentModel.DataAnnotations;

namespace Harc.Api.Modules.Identity.Data;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    [Required, MaxLength(50)]
    public string Role { get; set; } = "Employee"; // Admin, Manager, Employee
    
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}