using System.ComponentModel.DataAnnotations;

namespace Harc.Api.Modules.Identity.Data;

public class Role
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public Dictionary<string, string> DisplayName { get; set; } = new();

    public ICollection<User> Users { get; set; } = new List<User>();
}