using System.ComponentModel.DataAnnotations;
using Harc.Api.Common.Models;
namespace Harc.Api.Modules.Identity.Data;

public class Team : BaseEntity
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public Dictionary<string, string> DisplayName { get; set; } = new();
}