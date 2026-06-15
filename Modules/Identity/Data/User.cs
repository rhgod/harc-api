using System.ComponentModel.DataAnnotations;

namespace Harc.Api.Modules.Identity.Data;

public enum UserStatus
{
    Active = 1,
    Passive = 2,
    Terminated = 3,
    Contractor = 4
}

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string FullName { get; set; } = string.Empty;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public int TitleId { get; set; }
    public Title Title { get; set; } = null!;
    
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
    
    public Guid? ManagerId { get; set; }
    public User? Manager { get; set; }
    
    public UserStatus Status { get; set; } = UserStatus.Active;
    
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}

