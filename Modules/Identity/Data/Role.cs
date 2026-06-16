using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;

namespace Harc.Api.Modules.Identity.Data;

public class Role : BaseEntity
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public Dictionary<string, string> DisplayName { get; set; } = new();

    public ICollection<User> Users { get; set; } = new List<User>();
}