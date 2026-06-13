namespace Harc.Api.Modules.Identity.Features.GetMe;

public class GetMeResponse
{
    public string Message { get; set; } = string.Empty;
    public string? InternalUserId { get; set; }
    public string? UserEmail { get; set; }
    public string? AssignedRole { get; set; }
    public Dictionary<string, string>? AssignedRoleDisplayName { get; set; }
    public string? Avatar { get; set; }
}